using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ChatInterface {
    CircularBuffer buffer;
    StringBuilder inputBuilder;
    StringBuilder screenBuilder;

    public event Action<string> OnInput = delegate { };

    public ChatInterface(int width, int height) {
        buffer = new CircularBuffer(width, height - 1);
        inputBuilder = new StringBuilder();
        screenBuilder = new StringBuilder();
    }

    public void PushMessage(string message) {
        buffer.Push(message);
    }

    public string Update() {
        string result = null;
        while (Console.KeyAvailable) {
            var c = Console.ReadKey(true);
            if (c.Key == ConsoleKey.Enter) {
                result = inputBuilder.ToString();
                buffer.Push(result);
                OnInput(result);
                inputBuilder.Clear();
            } else if (c.Key == ConsoleKey.Backspace) {
                if (inputBuilder.Length > 0) {
                    inputBuilder.Remove(inputBuilder.Length - 1, 1);
                }
            } else {
                if (inputBuilder.Length < buffer.Width - 1) {
                    inputBuilder.Append(c.KeyChar);
                }
            }
        }

        screenBuilder.Clear();
        for (int i = 0; i < buffer.Height; i++) {
            if (i < buffer.Count) {
                screenBuilder.Append(buffer[i]);
                for (int j = buffer[i].Length; j < buffer.Width; j++) {
                    screenBuilder.Append(' ');
                }
            } else {
                screenBuilder.AppendLine();
            }
        }

        screenBuilder.Append(inputBuilder);
        for (int j = inputBuilder.Length; j < buffer.Width - 1; j++) {
            screenBuilder.Append(' ');
        }

        Console.CursorVisible = false;
        Console.SetCursorPosition(0, 0);
        Console.Write(screenBuilder.ToString());
        Console.SetCursorPosition(inputBuilder.Length, Console.WindowHeight - 1);
        Console.CursorVisible = true;
        return result;
    }
}
