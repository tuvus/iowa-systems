using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GraphFile {
    enum FileStartingVariables {
        PointSize,
        GraphSize,
        MaxValue,
    }
    public string path;


    public GraphFile(string fileName, int pointSize, Color32[] colors) {
        path = fileName;
        if (File.Exists(path))
            File.Delete(path);
        using (BufferedStream stream = new BufferedStream(new FileStream(path, FileMode.CreateNew), 3 * 4 + colors.Length * 3)) {
            using (BinaryWriter writer = new BinaryWriter(stream)) {
                writer.Write(pointSize);
                writer.Write(0);
                writer.Write(0);
                for (int i = 0; i < colors.Length; i++) {
                    writer.Write(colors[i].r);
                    writer.Write(colors[i].g);
                    writer.Write(colors[i].b);
                }
            }
        }
    }

    /// <summary>
    /// Adds a list of points to the population File
    /// </summary>
    public void AddPoints(int[] points) {
        int tempMax = int.MinValue;
            using (BufferedStream stream = new BufferedStream(new FileStream(path, FileMode.Append), points.Length * 4)) {
            using (BinaryWriter writer = new BinaryWriter(stream)) {
                for (int i = 0; i < points.Length; i++) {
                    writer.Write(points[i]);
                    tempMax = Mathf.Max(tempMax, points[i]);
                }
            }
        }
        SetMax(tempMax);
        SetGraphSize(GetGraphSize() + 1);
    }

    /// <summary>
    /// Sets the max population to max if it is greater
    /// </summary>
    /// <param name="max">The max population point in the graph</param>
    void SetMax(int max) {
        int fileMax = GetMax();
        if (fileMax < max) {
            SetStartingInt(max, (int)FileStartingVariables.MaxValue);
        }
    }

    /// <summary>
    /// Returns the highest population point in the file
    /// </summary>
    public int GetMax() {
        return GetStartingInt((int)FileStartingVariables.MaxValue);
    }

    /// <summary>
    /// Sets the size of the graph in the file to graphSIze
    /// </summary>
    void SetGraphSize(int graphSize) {
        SetStartingInt(graphSize, (int)FileStartingVariables.GraphSize);
    }

    /// <summary>
    /// Gets the name of the graph
    /// </summary>
    /// <returns>size of the graph</returns>
    public string GetGraphName() {
        return path;
    }

    /// <summary>
    /// Gets the size of the graph
    /// </summary>
    /// <returns>size of the graph</returns>
    public int GetGraphSize() {
        return GetStartingInt((int)FileStartingVariables.GraphSize);
    }

    /// <returns>Returns the number of points in each column</returns>
    public int GetPointSize() {
        return GetStartingInt((int)FileStartingVariables.PointSize);
    }

    public Color32[] GetSpeciesColors() {
        int pointSize = GetPointSize();
        Color32[] speciesColors = new Color32[pointSize];
        using (FileStream stream = new FileStream(path, FileMode.Open)) {
            stream.Seek(3 * 4, SeekOrigin.Begin);
            using (BinaryReader reader = new BinaryReader(stream)) {
                for (int i = 0; i < pointSize; i++) {
                    speciesColors[i] = new Color32(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(),255);
                }
            }
        }
        return speciesColors;
    }

    /// <summary>
    /// Gets an array of points from the population file at position location
    /// </summary>
    /// <param name="points">The array to store the points in</param>
    /// <param name="location">The position to get them from</param>
    public void GetPoints(int[] points, long location) {
        int pointSize = GetPointSize();
        using (BufferedStream stream = new BufferedStream(new FileStream(path, FileMode.Open),points.Length * 4) ) {
            stream.Seek(((location * points.Length) + 3) * 4 + pointSize * 3, SeekOrigin.Begin);
            using (BinaryReader reader = new BinaryReader(stream)) {
                for (int i = 0; i < points.Length; i++) {
                    points[i] = reader.ReadInt32();
                }
            }
        }
    }


    /// <summary>
    /// Sets the starting int in position position to value
    /// </summary>
    void SetStartingInt(int value, int position) {
        using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Write)) {
            using (BinaryWriter writer = new BinaryWriter(stream)) {
                writer.Seek(position * 4, SeekOrigin.Begin);
                writer.Write(value);
            }
        }
    }

    /// <summary>
    /// Returns the starting int at position
    /// </summary>
    int GetStartingInt(int position) {
        int value;
        using (FileStream stream = new FileStream(path, FileMode.Open)) {
            using (BinaryReader reader = new BinaryReader(stream)) {
                stream.Seek(position * 4, SeekOrigin.Begin);
                value = reader.ReadInt32();
            }
        }
        return value;
    }
}
