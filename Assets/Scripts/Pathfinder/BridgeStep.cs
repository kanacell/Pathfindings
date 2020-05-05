using System;
using System.Collections.Generic;

public class BridgeStep : System.IComparable<BridgeStep>
{
    #region Public Methods
    public BridgeStep(Bridge _Bridge, int _WeightToReach)
    {
        m_Bridge = _Bridge;
        m_Weight = _WeightToReach;
    }

    public BridgeStep MakeExtensionWith(Bridge _NextBridge, int _WeightToReach)
    {
        BridgeStep extension = new BridgeStep(_NextBridge, m_Weight + _WeightToReach);
        extension.m_Previous = this;
        return extension;
    }

    public void MarkAsSolution(int _WeightToReachGoal)
    {
        m_ReachedEnd = true;
        m_Weight += _WeightToReachGoal;
    }

    public Path CreateCompletePath(GridClustered _GridClustered, Dictionary<Tile, Path> _AllFirstParts, Dictionary<Tile, Path> _AllLastParts)
    {
        // init path with the last part which connect the last bridge with the goal
        Path path = new Path(_AllLastParts[m_Bridge.End]);

        // Save the current bridge to reach
        Bridge bridgeToReach = m_Bridge;

        // move backward in the bridgeStep list
        BridgeStep step = this.m_Previous;

        // Each iteration, merge the right path before the current path

        while (step != null)
        {
            Cluster cluster = _GridClustered.GetClusterOfTile(bridgeToReach.Start);
            Bridge previousBridge = step.Bridge;

            Path partPath = bridgeToReach.GetPathToBridgeBeginningWith(previousBridge.End);
            path.MergeBefore(partPath.ReversedPath());

            step = step.m_Previous;
            bridgeToReach = previousBridge;
        }
        path.MergeBefore(_AllFirstParts[bridgeToReach.Start]);

        return path;
    }

    public int CompareTo(BridgeStep _Other)
    {
        return m_Weight.CompareTo(_Other.m_Weight);
    }
    #endregion

    #region Getters/Setters
    public Bridge Bridge
    {
        get
        {
            return m_Bridge;
        }
    }

    public bool ReachedGoal
    {
        get
        {
            return m_ReachedEnd;
        }
    }
    #endregion

    #region Private Attributes
    private BridgeStep m_Previous = null;
    private Bridge m_Bridge = default;
    private int m_Weight = 0;
    private bool m_ReachedEnd = false;
    #endregion

}