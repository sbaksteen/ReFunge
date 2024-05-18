using System.Text;
using System.Text.RegularExpressions;
using ReFunge.Data.Values;

namespace ReFunge.Data;

internal class FungeSpace
{
    private readonly Dictionary<FungeVector, int> _data = [];
    public int Dim;

    private int[] _minCoords;
    private int[] _maxCoords;
    private readonly List<Dictionary<int, int>> _hists = [];

    private long _cellCount;

    public FungeSpace(int dim) 
    {
        Dim = dim;
        _minCoords = new int[dim];
        _maxCoords = new int[dim];
        for (var i = 0; i < dim; i++)
        {
            _minCoords[i] = int.MaxValue;
            _maxCoords[i] = int.MinValue;
            _hists.Add([]);
        }
    }

    internal FungeVector MinCoords => new(_minCoords);

    internal FungeVector MaxCoords => new(_maxCoords);

    private void UpdateMin(int d)
    {
        if (_hists[d].Count == 0)
        {
            _minCoords[d] = int.MaxValue;
            return;
        }
        _minCoords[d] = _hists[d].Keys.Min();
    }

    private void UpdateMax(int d)
    {
        if (_hists[d].Count == 0)
        {
            _maxCoords[d] = int.MinValue;
            return;
        }
        _maxCoords[d] = _hists[d].Keys.Max();
    }

    private void RemoveCell(FungeVector vector)
    {
        if (!_data.ContainsKey(vector))
        {
            return;
        }
        _cellCount--;
        _data.Remove(vector);
        for (var i = 0; i < Dim; i++)
        {
            var v = vector[i];
            _hists[i][v]--;
            if (_hists[i][v] != 0) continue;
            _hists[i].Remove(v);

            // Check for new lower/upper bounds
            if (v == _minCoords[i])
            {
                UpdateMin(i);
            }
            if (v == _maxCoords[i])
            {
                UpdateMax(i);
            }
        }
    }

    private void NewCell(FungeVector vector, FungeInt value)
    {
        _cellCount++;
        _data[vector] = value;
        for (var i = 0; i < Dim; i++)
        {
            var v = vector[i];
            if (!_hists[i].ContainsKey(v))
            {
                _hists[i][v] = 0;

                // Check for new lower/upper bounds
                if (v < _minCoords[i])
                {
                    _minCoords[i] = v;
                }
                if (v > _maxCoords[i])
                {
                    _maxCoords[i] = v;
                }
            }
            _hists[i][v]++;
        }
    }

    public bool OutOfBounds(FungeVector vector)
    {
        for (var i = 0; i < Dim; i++)
        {
            if (vector[i] < _minCoords[i] || vector[i] > _maxCoords[i])
            {
                return true;
            }
        }
        return false;
    }

    public FungeVector Wrap(FungeVector position, FungeVector delta)
    {
        var steps = int.MaxValue;
        for (var i = 0; i < Dim; i++)
        {
            steps = delta[i] switch
            {
                > 0 => Math.Min(steps, (position[i] - _minCoords[i]) / delta[i]),
                < 0 => Math.Min(steps, (position[i] - _maxCoords[i]) / delta[i]),
                _ => steps
            };
        }
        return position - delta * steps;
    }

    public FungeVector LoadFromReader(FungeVector position, TextReader reader, bool binary = false)
    {
        var x = position[0];
        var y = position[1];
        var z = position[2];
        var maxX = 0;
        var maxY = 0;
        var c = reader.Read();
        while (c != -1)
        {
            if (!binary)
            {
                switch (c)
                {
                    case '\r':
                    case '\n' when Dim < 2:
                    case '\f' when Dim < 3:
                        c = reader.Read();
                        continue;
                    case '\n' when Dim > 1:
                    {
                        if (x - position[0] > maxX)
                        {
                            maxX = x - position[0];
                        }

                        x = position[0];
                        y++;
                        c = reader.Read();
                        continue;
                    }
                    case '\f' when Dim > 2:
                    {
                        if (y - position[1] + 1 > maxY)
                        {
                            maxY = y - position[1] + 1;
                        }

                        x = position[0];
                        y = position[1];
                        z++;
                        c = reader.Read();
                        continue;
                    }
                }
            }

            if (c != ' ')
            {
                this[x, y, z] = c;
            }

            c = reader.Read();
            x++;
        }
        reader.Close();
        if (x - position[0] > maxX)
        {
            maxX = x - position[0];
        }
        if (y - position[1] + 1 > maxY)
        {
            maxY = y - position[1];
        }
        var maxZ = z - position[2] + 1;
        return Dim switch
        {
            > 2 => new FungeVector(maxX, maxY, maxZ),
            > 1 => new FungeVector(maxX, maxY),
            _ => new FungeVector(maxX)
        };
    }

    public FungeVector LoadString(FungeVector position, string data, bool binary = false)
    {
        var reader = new StringReader(data);
        return LoadFromReader(position, reader, binary);
    }

    public FungeVector LoadFile(FungeVector position, string path, bool binary = false)
    {
        var detectEncoding = true;
        var reader = new StreamReader(path, detectEncoding);
        return LoadFromReader(position, reader, binary);
    }

    public void WriteToWriter(FungeVector position, FungeVector size, TextWriter writer, bool linear = false)
    {
        if (size.Dim > 3)
        {
            writer.Close();
            throw new ArgumentException("Can't output 4D or higher Funge-Space!");
        }
        for (var i = 0; i < size.Dim; i++)
        {
            if (size[i] <= 0)
            {
                writer.Close();
                throw new ArgumentException("Size can't be non-positive!");
            }
        }
        var end = position + size;
        var x = position[0];
        var y = position[1];
        var z = position[2];
        var feedsWithoutContent = 0;
        while (z != end[2] || Dim < 3)
        {
            var linesWithoutContent = 0;
            while (y != end[1] || Dim < 2)
            {
                while (x != end[0])
                {
                    var spaces = 0;
                    var c = this[x, y, z];
                    while (linear && c == ' ' && x < end[0])
                    {
                        spaces++;
                        c = this[++x, y, z];
                    }

                    if (x == end[0])
                    {
                        continue;
                    }
                    if (linear && x < end[0])
                    {
                        if (feedsWithoutContent > 0)
                        {
                            writer.Write(new string('\f', feedsWithoutContent));
                            feedsWithoutContent = 0;
                        }
                        if (linesWithoutContent > 0)
                        {
                            writer.Write(new string('\n', linesWithoutContent));
                            linesWithoutContent = 0;
                        }
                        if (spaces > 0)
                        {
                            writer.Write(new string(' ', spaces));
                        }
                    }
                    writer.Write((char)this[x++,y,z]);
                }

                if (Dim < 2)
                {
                    return;
                }
                    
                linesWithoutContent++;

                y++;
                x = position[0];
                if (!linear && y != end[1])
                {
                    writer.Write('\n');
                }
            }

            if (Dim < 3)
            {
                return;
            }
                
            feedsWithoutContent++;
                
            z++;
            y = position[1];
            if (!linear && z != end[2])
            {
                writer.Write('\f');
            }
        }
    }
        
    public string WriteToString(FungeVector position, FungeVector size, bool linear = false)
    {
        var writer = new StringWriter();
        WriteToWriter(position, size, writer, linear);
        return writer.ToString();
    }

    public void WriteToFile(FungeVector position, FungeVector size, string filename, bool linear = false)
    {
        var writer = new StreamWriter(filename);
        WriteToWriter(position, size, writer, linear);
        writer.Close();
    }

    internal FungeInt this[params int[] ints]
    {
        get => this[new FungeVector(ints)];
        set => this[new FungeVector(ints)] = value;
    }

    public FungeInt this[FungeVector vector]
    {
        get
        {
            if (vector.Size > Dim)
            {
                for (var i = Dim; i < vector.Size; i++)
                {
                    if (vector[i] != 0)
                    {
                        throw new ArgumentException("Vector is too large for this space!");
                    }
                }
            }

            // If there is no record, return 32 (space character)
            return _data.GetValueOrDefault(vector, 32);

        }
        set
        {
            if (vector.Size > Dim)
            {
                for (var i = Dim; i < vector.Size; i++)
                {
                    if (vector[i] != 0)
                    {
                        throw new ArgumentException("Vector is too large for this space!");
                    }
                }
            }

            if (value == 32)
            {
                // If a space is written, remove the record
                RemoveCell(vector);
                return;
            }

            if (!_data.ContainsKey(vector))
            {
                NewCell(vector, value);
                return;
            }
            _data[vector] = value;
        }
    }
}