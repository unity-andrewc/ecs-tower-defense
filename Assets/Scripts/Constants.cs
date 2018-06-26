using Unity.Mathematics;

public static class Constants
{
    public static class Enemy
    {
        public static float COOLDOWN = 2.5f;
        //TODO : this could be changing in order to make game more difficult?
        public static float WAVE_COOLDOWN = 5.0f;

        public static float3 BODY_SCALE = new float3(0.1f, 0.02f, 0.1f);
        public static float3 HEAD_SCALE = new float3(0.06f, 0.04f, 0.06f);
    }
}
