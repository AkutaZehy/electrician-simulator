using System;

public static class SignalProcessor
{
    public static int[] Process(int[] input, string type)
    {
        int[] result = (int[])input.Clone();
        int len = result.Length;

        switch (type)
        {
            case "ADD": 
                for (int i = 0; i < len; i++) result[i] = Math.Min(5, result[i] + 1); 
                break;
            case "MUL": 
                for (int i = 0; i < len; i++) result[i] = Math.Min(5, result[i] * 2); 
                break;
            case "INV": 
                for (int i = 0; i < len; i++) result[i] = (result[i] == 0) ? 5 : 0; 
                break;
            case "DLY": 
                int tmp = result[len - 1];
                for (int i = len - 1; i > 0; i--) result[i] = result[i - 1];
                result[0] = tmp;
                break;
            case "OSC":
                int[] osc = { 1, 3, 5, 3 };
                for (int i = 0; i < len; i++) result[i] = result[i] == 0 ? 0 : osc[i % 4]; 
                break;
        }
        return result;
    }

    public static int[] ApplyDecay(int[] input)
    {
        int[] result = (int[])input.Clone();
        for (int i = 0; i < result.Length; i++)
            result[i] = Math.Max(0, result[i] - 1);
        return result;
    }
}