using Unity.Mathematics;

public static class Grid
{
    public static void Configure(int2 numCells, float2 cellSize, float3 center)
    {
        s_NumCells = numCells;
        s_CellSize = cellSize;
        s_BottomLeft = center - 0.5f * new float3(numCells.x * cellSize.x, 0.0f, numCells.y * cellSize.y);
    }

    public static int2 NumCells => s_NumCells;
    public static float2 CellSize => s_CellSize;

    public static int2 ConvertToGridIndex(float3 position)
    {
        position -= s_BottomLeft;
        return new int2((int)((float)position.x / s_CellSize.x), (int)((float)position.z / s_CellSize.y));
    }

    public static float3 ConvertToWorldPosition(int2 gridIndex)
    {
        float3 scaledByCellSize = new float3((float)gridIndex.x * s_CellSize.x, 0.0f, (float)gridIndex.y * s_CellSize.y);
        return scaledByCellSize + s_BottomLeft + new float3(0.5f * s_CellSize.x, 0.0f, 0.5f * s_CellSize.y);
    }

    private static int2 s_NumCells;
    private static float2 s_CellSize;
    private static float3 s_BottomLeft;
}
