using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GraphFileTest : MonoBehaviour {
    enum FileStartingVariables {
        PointSize,
        GraphSize,
        MaxValue,
    }
    public string path;

    public void SetupGraphFileManager(int pointSize) {
        if (File.Exists(path))
            File.Delete(path);
        using (FileStream stream = new FileStream(path, FileMode.CreateNew)) {
            using (BinaryWriter writer = new BinaryWriter(stream)) {
                writer.Write(pointSize);
                writer.Write(0);
                writer.Write(0);
            }
        }
    }

    /// <summary>
    /// Adds a list of points to the population File
    /// </summary>
    public void AddPoints(int[] points) {
        int tempMax = int.MinValue;
        using (FileStream stream = new FileStream(path, FileMode.Append)) {
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
    /// Gets the size of the graph
    /// </summary>
    /// <returns>size of the graph</returns>
    public int GetGraphSize() {
        return  GetStartingInt((int)FileStartingVariables.GraphSize);
    }

    /// <returns>Returns the number of points in each column</returns>
    public int GetPointSize() {
        return GetStartingInt((int)FileStartingVariables.PointSize);
    }

    /// <summary>
    /// Gets an array of points from the population file at position location
    /// </summary>
    /// <param name="points">The array to store the points in</param>
    /// <param name="location">The position to get them from</param>
    public void GetPoints(int[] points, long location) {
        using (FileStream stream = new FileStream(path, FileMode.Open)) {
            stream.Seek(((location * points.Length) + 3) * 4, SeekOrigin.Begin);
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
