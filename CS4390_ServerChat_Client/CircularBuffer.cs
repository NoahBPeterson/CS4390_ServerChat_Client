using System;
using System.Collections.Generic;

public class CircularBuffer {
    public int Width { get; private set; }
    public int Height { get; private set; }

    int head;
    public int Count { get; private set; }

    string[] buffer;

    public CircularBuffer(int width, int height) {
        Width = width;
        Height = height;

        buffer = new string[Height];
    }

    int GetRow(int index) {
        return (head + index) % Height;
    }

    public string this[int index] {
        get {
            lock (buffer) {
                return buffer[GetRow(index)];
            }
        }
    }

    public void Push(string message) {
        lock (buffer) {
            int row = GetRow(Count);
            if (message.Length <= Width) {
                buffer[row] = message;
            } else {
                buffer[row] = message.Substring(0, Width);
            }

            if (Count == Height) {
                head = (head + 1) % Height;
            } else {
                Count++;
            }
        }
    }
}
