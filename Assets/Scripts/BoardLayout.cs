using UnityEngine;

public class BoardLayout : MonoBehaviour
{
    public LayoutRows[] allRows;
    public Gem[,] GetLayout(int width, int height)
    {
        Gem[,] theLayout = new Gem[width, height];

        if (allRows == null || allRows.Length == 0)
        {
            return theLayout; 
        }

        if (allRows.Length > height)
        {
            Debug.LogWarning("BoardLayout: có " + allRows.Length + " hàng nhưng Board chỉ cao " + height
                + " -> các hàng trên cùng bị bỏ qua.");
        }

        for (int row = 0; row < allRows.Length; row++)
        {
            if (allRows[row] == null || allRows[row].gemsInRow == null)
            {
                continue;
            }

            int y = allRows.Length - 1 - row;
            if (y < 0 || y >= height)
            {
                continue;
            }

            if (allRows[row].gemsInRow.Length > width)
            {
                Debug.LogWarning("BoardLayout: hàng " + row + " có " + allRows[row].gemsInRow.Length
                    + " ô nhưng Board chỉ rộng " + width + " -> các ô thừa bị bỏ qua.");
            }

            for (int x = 0; x < allRows[row].gemsInRow.Length && x < width; x++)
            {
                theLayout[x, y] = allRows[row].gemsInRow[x];
            }
        }

        return theLayout;
    }
}

[System.Serializable]
public class LayoutRows
{
    public Gem[] gemsInRow;
}