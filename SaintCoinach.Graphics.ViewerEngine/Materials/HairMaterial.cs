﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Direct3D11;

namespace SaintCoinach.Graphics.Materials {
    public class HairMaterial : XivMaterial {
        #region Fields
        private ShaderResourceView _Mask;
        private ShaderResourceView _Normal;
        #endregion

        #region Properties
        public ShaderResourceView Mask { get { return _Mask; } }
        public ShaderResourceView Normal { get { return _Normal; } }

        public new Effects.HairEffect Effect { get { return (Effects.HairEffect)base.Effect; } }
        #endregion

        #region Constructor
        public HairMaterial(Device device, Assets.MaterialVersion material)
            : base(device, material, EffectCache.Get(device).HairEffect) {


            var texCache = TextureCache.Get(device);
            foreach (var texParam in material.ParameterMappings) {
                var tex = material.Textures[texParam.Index];

                string target;
                if (!Utilities.Effect.CharacterParameterMap.TryGetValue(texParam.Id, out target)) {
                    System.Diagnostics.Trace.WriteLine(string.Format("Failed to map parameter with id {0:X8}h (on {1})", texParam.Id, tex.Path));
                    continue;
                }

                switch (target) {
                    case "Normal":
                        _Normal = texCache.GetResource(tex);
                        break;
                    case "Mask":
                        _Mask = texCache.GetResource(tex);
                        break;
                    default:
                        System.Diagnostics.Trace.WriteLine(string.Format("Unknown parameter {0} for {1}.", texParam.Id, tex.Path));
                        break;
                }
            }

            CurrentTechniqueName = Utilities.Effect.GetTechniqueName(material);
        }
        #endregion

        #region Apply
        public override void Apply() {
            Effect.Normal = this.Normal;
            Effect.Mask = this.Mask;
        }
        #endregion
    }
}