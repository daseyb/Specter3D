using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.ThirdParty.Spriter2Unity.Editor.Spriter;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Unity
{
    public class PrefabBuilder
    {
        public GameObject MakePrefab(Entity entity)
        {
            foreach (var animation in entity.Animations)
            {

            }
            throw new NotImplementedException();
        }
    }
}
