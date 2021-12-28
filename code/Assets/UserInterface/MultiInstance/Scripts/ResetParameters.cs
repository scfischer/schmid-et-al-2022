using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UserInterface.MultiInstance
{
public class ResetParameters : InstanceConsumer
{
    public void ResetToDefault()
    {
        Alveolus.instanceParameters.ResetToDefaults();
    }

    protected override void HandleSimulationOutputChanged()
    {
    }
}
}

