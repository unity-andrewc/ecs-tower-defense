using Unity.Mathematics;

public class GridBitfield
{
    public GridBitfield()
    {
        int numBits = Grid.NumCells.x * Grid.NumCells.y;
        int numInts = numBits / 32;
        if ((numInts * 32) < numBits)
            ++numInts;
        m_Bits = new int[numInts];
    }

    public bool this[int x, int y]
    {
        get
        {
            int arrayIndex, bitIndex;
            GetIndices(x, y, out arrayIndex, out bitIndex);
            return 0 != (m_Bits[arrayIndex] & (1 << bitIndex));
        }
        set
        {
            int arrayIndex, bitIndex;
            GetIndices(x, y, out arrayIndex, out bitIndex);
            if (value)
                m_Bits[arrayIndex] |= 1 << bitIndex;
            else
                m_Bits[arrayIndex] &= ~(1 << bitIndex);
        }
    }
    
    public bool this[int2 coords]
    {
        get
        {
            return this[coords.x, coords.y];
        }
        set
        {
            this[coords.x, coords.y] = value;
        }
    }

    private void GetIndices(int x, int y, out int arrayIndex, out int bitIndex)
    {
        int overallIndex = x + y * Grid.NumCells.x;
        arrayIndex = overallIndex / 32;
        bitIndex = overallIndex - arrayIndex * 32;
    }

    int[] m_Bits;
}
