using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using ReFunge.Data.Values;

namespace ReFunge.Data
{
    internal class FungeSpace
    {
        private readonly Dictionary<FungeVector, int> _Data = [];
        public int Dim;

        private int[] _minCoords;
        private int[] _maxCoords;
        private readonly List<Dictionary<int, int>> Hists = [];

        private long CellCount;

        public FungeSpace(int dim) 
        {
            Dim = dim;
            _minCoords = new int[dim];
            _maxCoords = new int[dim];
            for (int i = 0; i < dim; i++)
            {
                _minCoords[i] = int.MaxValue;
                _maxCoords[i] = int.MinValue;
                Hists.Add([]);
            }
        }

        internal FungeVector MinCoords => new(_minCoords);

        internal FungeVector MaxCoords => new(_maxCoords);

        private void UpdateMin(int d)
        {
            if (Hists[d].Count == 0)
            {
                _minCoords[d] = int.MaxValue;
                return;
            }
            _minCoords[d] = Hists[d].Keys.Min();
        }

        private void UpdateMax(int d)
        {
            if (Hists[d].Count == 0)
            {
                _maxCoords[d] = int.MinValue;
                return;
            }
            _maxCoords[d] = Hists[d].Keys.Max();
        }

        private void RemoveCell(FungeVector vector)
        {
            if (!_Data.ContainsKey(vector))
            {
                return;
            }
            CellCount--;
            _Data.Remove(vector);
            for (int i = 0; i < Dim; i++)
            {
                int v = vector[i];
                Hists[i][v]--;
                if (Hists[i][v] == 0)
                {
                    Hists[i].Remove(v);

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
        }

        private void NewCell(FungeVector vector, FungeInt value)
        {
            CellCount++;
            _Data[vector] = value;
            for (int i = 0; i < Dim; i++)
            {
                int v = vector[i];
                if (!Hists[i].ContainsKey(v))
                {
                    Hists[i][v] = 0;

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
                Hists[i][v]++;
            }
        }

        public bool OutOfBounds(FungeVector vector)
        {
            for (int i = 0; i < Dim; i++)
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
            int steps = int.MaxValue;
            for (int i = 0; i < Dim; i++)
            {
                if (delta[i] > 0)
                {
                    steps = Math.Min(steps, (position[i] - _minCoords[i]) / delta[i]);
                }
                else if (delta[i] < 0)
                {
                    steps = Math.Min(steps, (position[i] - _maxCoords[i]) / delta[i]);
                }
            }
            return position - delta * steps;
        }

        public FungeVector LoadCharacters(FungeVector position, char[] chars, bool binary = false)
        {
            int x = position[0];
            int y = position[1];
            int z = position[2];
            int maxX = 0;
            int maxY = 0;
            int maxZ = 0;
            foreach (char c in chars)
            {
                if ((c == '\r' && !binary) || (c == '\n' && Dim < 2 && !binary) || (c == '\f' && Dim < 3 && !binary))
                {
                    continue;
                }
                if (c == '\n' && !binary && Dim > 1)
                {
                    if (x - position[0] - 1 > maxX)
                    {
                        maxX = x - position[0] - 1;
                    }

                    x = position[0];
                    y++;
                    continue;
                }
                if (c == '\f' && !binary && Dim > 2)
                {
                    if (y - position[1] > maxY)
                    {
                        maxY = y - position[1];
                    }
                    x = position[0];
                    y = position[1];
                    z++;
                    continue;
                }

                if (c != ' ')
                {
                    this[x, y, z] = c;
                }

                x++;
            }
            if (x - position[0] - 1 > maxX)
            {
                maxX = x - position[0] - 1;
            }
            if (y - position[1] > maxY)
            {
                maxY = y - position[1];
            }
            maxZ = z - position[2];
            return Dim switch
            {
                > 2 => new FungeVector(maxX, maxY, maxZ),
                > 1 => new FungeVector(maxX, maxY),
                _ => new FungeVector(maxX)
            };
        }

        public FungeVector LoadString(FungeVector position, string data, bool binary = false)
        {
            return LoadCharacters(position, data.ToCharArray(), binary);
        }

        public FungeVector LoadFile(FungeVector position, string path, bool binary = false)
        {
            char[] chars;
            if (binary)
            {
                chars = File.ReadAllBytes(path).Select(b => (char)b).ToArray();
            } else
            {
                chars = File.ReadAllText(path).ToCharArray();
            }
            return LoadCharacters(position, chars, binary);
        }

        public string WriteToString(FungeVector position, FungeVector size, bool linear = false)
        {
            if (size.Dim > 3)
            {
                throw new ArgumentException("Can't output 4D or higher Funge-Space!");
            }
            for (int i = 0; i < size.Dim; i++)
            {
                if (size[i] < 0)
                {
                    throw new ArgumentException("Size can't be negative!");
                }
            }
            StringWriter writer = new StringWriter();
            FungeVector end = position + size;
            int x = position[0];
            int y = position[1];
            int z = position[2];
            while (x <= end[0] || y != end[1] || z != end[2])
            {
                if (x > end[0] && size.Dim > 1)
                {
                    x = position[0];
                    y++;
                    if (y > end[1] && size.Dim > 2)
                    {
                        y = position[1];
                        z++;
                        writer.Write('\f');
                        continue;
                    }
                    writer.Write('\n');
                    continue;
                }
                writer.Write((char)this[x++,y,z]);
            }
            string data = writer.ToString();
            if (linear)
            {
                data = Regex.Replace(data, "[\n\f ]+$|[\n ]+(?=\f)| +(?=\n)", "");
            }
            return data;
        }

        public void WriteToFile(FungeVector position, FungeVector size, string filename, bool linear = false)
        {
            File.WriteAllText(filename, WriteToString(position, size, linear));
        }

        internal FungeInt this[params int[] ints]
        {
            get
            {
                return this[new FungeVector(ints)];
            }
            set
            {
                this[new FungeVector(ints)] = value;
            }
        }

        public FungeInt this[FungeVector vector]
        {
            get
            {
                if (vector.Size > Dim)
                {
                    for (int i = Dim; i < vector.Size; i++)
                    {
                        if (vector[i] != 0)
                        {
                            throw new ArgumentException("Vector is too large for this space!");
                        }
                    }
                }

                if (_Data.TryGetValue(vector, out int value))
                {
                    return value;
                }

                // If there is no record, return 32 (space character)
                return 32;
            }
            set
            {
                if (vector.Size > Dim)
                {
                    for (int i = Dim; i < vector.Size; i++)
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

                if (!_Data.ContainsKey(vector))
                {
                    NewCell(vector, value);
                    return;
                }
                _Data[vector] = value;
            }
        }
    }
}
