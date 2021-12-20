using Unity.Mathematics;
using Unity.Collections;


public struct ZoneTestScript {
    public ZoneTestScript(float3 position) {
        this.position = position;
        neiboringZones = new NativeArray<int>(30,Allocator.Persistent);
    }

    public float3 position;
    public NativeArray<int> neiboringZones;
}