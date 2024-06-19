using ReFunge.Data.Values;

namespace ReFunge.Data;

/// <summary>
///     Represents a Funge-Space, a theoretically infinite n-dimensional grid of characters.
///     Provides amortized O(1) access to cells, amortized O(1) insertion and removal of cells,
///     O(1) access to the minimum and maximum bounds of the space, and O(1) wrapping of coordinates around the space.
/// </summary>
/// <remarks>
///     The "amortized" is doing a lot of work in that description. The performance of this class is highly dependent on
///     how often cells are removed such that the bounds need to be updated. Updating the bounds when a cell is removed
///     at the minimum or maximum bound (when this is the only cell at that bound) is best-case O(dth root of n), where d
///     is the dimension of the space and n is the number of cells in the space. In the worst case, this is O(n), when
///     the space is a line. Access is amortized O(1) in the sense of <see cref="Dictionary{TKey,TValue}"/>, with the
///     calculation of the vector hash code being O(d). All other operations are also O(d) due to similar considerations.
///     In practice, O(d) is essentially O(1) for most use cases. <br />
///     The space is implemented as a dictionary from FungeVectors to ints,
///     with a dictionary-based histogram for each dimension keeping track of the number of cells sharing each coordinate
///     in that dimension. This lets us quickly update the minimum and maximum bounds of the space when cells are added,
///     and only do so when necessary when cells are removed.
/// </remarks>
public class FungeSpace
{
    private readonly Dictionary<FungeVector, int> _data = [];
    private readonly List<Dictionary<int, int>> _hists = [];
    private readonly int[] _maxCoords;
    private readonly int[] _minCoords;

    private long _cellCount;

    /// <summary>
    ///     The dimensionality of the space.
    /// </summary>
    public int Dim;

    /// <summary>
    ///     Create a new Funge-Space with the given dimensionality.
    /// </summary>
    /// <param name="dim">The dimensionality of the space.</param>
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

    /// <summary>
    ///     The minimum bounds of the space. This is a vector containing the least coordinate in each dimension that has a
    ///     non-space character in it.
    /// </summary>
    public FungeVector MinBounds => new(_minCoords);

    /// <summary>
    ///     The maximum bounds of the space. This is a vector containing the greatest coordinate in each dimension that has a
    ///     non-space character in it.
    /// </summary>
    public FungeVector MaxBounds => new(_maxCoords);

    /// <summary>
    ///     Gets or sets the cell at the given coordinates in the space.
    /// </summary>
    /// <param name="ints">The coordinates of the cell.</param>
    /// <value>The value of the cell at the given coordinates.</value>
    /// <seealso cref="FungeSpace.this[FungeVector]" />
    public FungeInt this[params int[] ints]
    {
        get => this[new FungeVector(ints)];
        set => this[new FungeVector(ints)] = value;
    }

    /// <summary>
    ///     Gets or sets the cell at the given vector in the space. <br />
    ///     Getter: If no value has been set for the given vector, the space character (ASCII 32) is returned. <br />
    ///     Setter: If the value to be set is the space character, the cell is removed from the space if it already exists.
    ///     If the value is not the space character, the cell is added to the space if it does not already exist.
    ///     In either of these cases, the minimum and maximum bounds of the space are updated if necessary.
    /// </summary>
    /// <param name="vector">The coordinates of the cell.</param>
    /// <value>The value of the cell at the given coordinates.</value>
    /// <exception cref="ArgumentException">Thrown when the given vector is of a dimensionality greater than the space.</exception>
    public FungeInt this[FungeVector vector]
    {
        get
        {
            if (vector.Dim > Dim) throw new ArgumentException("Vector is too large for this space!");

            // If there is no record, return 32 (space character)
            return _data.GetValueOrDefault(vector, 32);
        }
        set
        {
            if (vector.Dim > Dim) throw new ArgumentException("Vector is too large for this space!");

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
        if (!_data.ContainsKey(vector)) return;
        _cellCount--;
        _data.Remove(vector);
        for (var i = 0; i < Dim; i++)
        {
            var v = vector[i];
            _hists[i][v]--;
            if (_hists[i][v] != 0) continue;
            _hists[i].Remove(v);

            // Check for new lower/upper bounds
            if (v == _minCoords[i]) UpdateMin(i);
            if (v == _maxCoords[i]) UpdateMax(i);
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
                if (v < _minCoords[i]) _minCoords[i] = v;
                if (v > _maxCoords[i]) _maxCoords[i] = v;
            }

            _hists[i][v]++;
        }
    }

    /// <summary>
    ///     Check if the given vector is out of bounds of the space. A vector is out of bounds if any of its coordinates are
    ///     less than the minimum bound or greater than the maximum bound in that dimension.
    /// </summary>
    /// <param name="vector">The vector to check.</param>
    /// <returns>True if the vector is out of bounds, false otherwise.</returns>
    public bool OutOfBounds(FungeVector vector)
    {
        for (var i = 0; i < Dim; i++)
            if (vector[i] < _minCoords[i] || vector[i] > _maxCoords[i])
                return true;
        return false;
    }

    /// <summary>
    ///     Wrap the given position around the space using the given delta. This is done by finding the number of steps
    ///     needed to reach the minimum or maximum bound in any dimension, and then moving the position by that many steps
    ///     in the opposite direction of the delta.
    /// </summary>
    /// <param name="position">The position to wrap.</param>
    /// <param name="delta">
    ///     The delta to wrap by. This is treated as the "forward" direction,
    ///     so we need to move in the opposite direction to wrap.
    /// </param>
    /// <returns>The wrapped position.</returns>
    public FungeVector Wrap(FungeVector position, FungeVector delta)
    {
        var steps = int.MaxValue;
        for (var i = 0; i < Dim; i++)
            steps = delta[i] switch
            {
                > 0 => Math.Min(steps, (position[i] - _minCoords[i]) / delta[i]),
                < 0 => Math.Min(steps, (position[i] - _maxCoords[i]) / delta[i]),
                _ => steps
            };
        return position - delta * steps;
    }

    /// <summary>
    ///     Load data from the given reader into space, starting at the given position.
    ///     If binary is true, newlines and form feeds are written verbatim and the data is written in a line.
    ///     Otherwise, newlines signal a new line of space and form feeds signal a new 3D layer of space.
    /// </summary>
    /// <param name="position">The position to start loading at.</param>
    /// <param name="reader">The reader to load data from.</param>
    /// <param name="binary">Whether to load the data in binary mode.</param>
    /// <returns>The size of the bounding box of space affected by the load.</returns>
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
                switch (c)
                {
                    case '\r':
                    case '\n' when Dim < 2:
                    case '\f' when Dim < 3:
                        c = reader.Read();
                        continue;
                    case '\n' when Dim > 1:
                    {
                        if (x - position[0] > maxX) maxX = x - position[0];

                        x = position[0];
                        y++;
                        c = reader.Read();
                        continue;
                    }
                    case '\f' when Dim > 2:
                    {
                        if (y - position[1] + 1 > maxY) maxY = y - position[1] + 1;

                        x = position[0];
                        y = position[1];
                        z++;
                        c = reader.Read();
                        continue;
                    }
                }

            if (c != ' ') this[x, y, z] = c;

            c = reader.Read();
            x++;
        }

        reader.Close();
        if (x - position[0] > maxX) maxX = x - position[0];
        if (y - position[1] + 1 > maxY) maxY = y - position[1];
        var maxZ = z - position[2] + 1;
        return Dim switch
        {
            > 2 => new FungeVector(maxX, maxY, maxZ),
            > 1 => new FungeVector(maxX, maxY),
            _ => new FungeVector(maxX)
        };
    }

    /// <summary>
    ///     Load the given string into space, starting at the given position. This is a wrapper around
    ///     <see cref="LoadFromReader" />.
    /// </summary>
    /// <seealso cref="LoadFromReader" />
    /// <param name="position">The position to start loading at.</param>
    /// <param name="data">The string to load.</param>
    /// <param name="binary">Whether to load the data in binary mode.</param>
    /// <returns>The size of the bounding box of space affected by the load.</returns>
    public FungeVector LoadString(FungeVector position, string data, bool binary = false)
    {
        var reader = new StringReader(data);
        return LoadFromReader(position, reader, binary);
    }

    /// <summary>
    ///     Load the data from the file at the given path into space, starting at the given position.
    ///     This is a wrapper around <see cref="LoadFromReader" />.
    /// </summary>
    /// <seealso cref="LoadFromReader" />
    /// <param name="position">The position to start loading at.</param>
    /// <param name="path">The path to the file to load.</param>
    /// <param name="binary">Whether to load the data in binary mode.</param>
    /// <returns>The size of the bounding box of space affected by the load.</returns>
    public FungeVector LoadFile(FungeVector position, string path, bool binary = false)
    {
        var detectEncoding = true;
        var reader = new StreamReader(path, detectEncoding);
        return LoadFromReader(position, reader, binary);
    }

    /// <summary>
    ///     Write the data in the space to the given writer, starting at the given position and with the given size.
    ///     Newline characters are written after each line of data, and form feed characters are written after each layer of
    ///     data.
    ///     <br />
    ///     If linear is true, no form feeds are written before the end of the data, no newlines are written before a
    ///     form feed, and no spaces are written before a newline, condensing the data losslessly.
    /// </summary>
    /// <param name="position">The position to start writing from.</param>
    /// <param name="size">The size of the data to write.</param>
    /// <param name="writer">The writer to write the data to.</param>
    /// <param name="linear">Whether to write the data in linear mode.</param>
    /// <exception cref="ArgumentException">
    ///     Thrown when any elements of the size vector are non-positive,
    ///     or when the size vector has more than 3 dimensions.
    /// </exception>
    public void WriteToWriter(FungeVector position, FungeVector size, TextWriter writer, bool linear = false)
    {
        if (size.Dim > 3)
        {
            writer.Close();
            throw new ArgumentException("Can't output 4D or higher Funge-Space!");
        }

        for (var i = 0; i < size.Dim; i++)
            if (size[i] <= 0)
            {
                writer.Close();
                throw new ArgumentException("Size can't be non-positive!");
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

                    if (x == end[0]) continue;
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

                        if (spaces > 0) writer.Write(new string(' ', spaces));
                    }

                    writer.Write((char)this[x++, y, z]);
                }

                if (Dim < 2) return;

                linesWithoutContent++;

                y++;
                x = position[0];
                if (!linear && y != end[1]) writer.Write('\n');
            }

            if (Dim < 3) return;

            feedsWithoutContent++;

            z++;
            y = position[1];
            if (!linear && z != end[2]) writer.Write('\f');
        }
    }

    /// <summary>
    ///     Write the data in the space to a string, starting at the given position and with the given size.
    ///     This is a wrapper around <see cref="WriteToWriter" />.
    /// </summary>
    /// <seealso cref="WriteToWriter" />
    /// <param name="position">The position to start writing from.</param>
    /// <param name="size">The size of the data to write.</param>
    /// <param name="linear">Whether to write the data in linear mode.</param>
    /// <returns>The string representation of the data in the space.</returns>
    public string WriteToString(FungeVector position, FungeVector size, bool linear = false)
    {
        var writer = new StringWriter();
        WriteToWriter(position, size, writer, linear);
        return writer.ToString();
    }

    /// <summary>
    ///     Write the data in the space to the file at the given path, starting at the given position and with the given size.
    ///     This is a wrapper around <see cref="WriteToWriter" />.
    /// </summary>
    /// <param name="position">The position to start writing from.</param>
    /// <param name="size">The size of the data to write.</param>
    /// <param name="filename">The path to the file to write the data to.</param>
    /// <param name="linear">Whether to write the data in linear mode.</param>
    public void WriteToFile(FungeVector position, FungeVector size, string filename, bool linear = false)
    {
        var writer = new StreamWriter(filename);
        WriteToWriter(position, size, writer, linear);
        writer.Close();
    }

    public FungeSpace Clone()
    {
        FungeSpace r = new FungeSpace(Dim);
        foreach (var t in _data)
        {
            r[t.Key] = t.Value;
        }

        return r;
    }
}