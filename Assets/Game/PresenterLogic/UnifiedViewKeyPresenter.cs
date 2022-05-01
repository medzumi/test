using System;
using System.Collections.Generic;
using System.Linq;
using Game.CoreLogic;
using Unity;
using UnityEngine;

namespace Game.PresenterLogic
{
    public class UnifiedViewKeyPresenter : AbstractEcsPresenter<UnifiedViewKeyPresenter, UnifiedViewKeyComponent>
    {
        public bool IsRethrowExceptionOrCallDefault;
        
        [MonoViewModelKeyProperty] public string DefaultViewModelKey;
        public List<Composition> Compositions = new List<Composition>();

        [Serializable]
        public struct Composition
        {
            public string UnifiedViewKey;
            [MonoViewModelKeyProperty] public string ViewModelKey;
            [PresenterKeyProperty] public string PresenterKey;
        }

        public override void Initialize(EcsPresenterData ecsPresenterData)
        {
            base.Initialize(ecsPresenterData);
        }

        protected override void Update(UnifiedViewKeyComponent data)
        {
            base.Update(data);
            try
            {
                var composiotion =
                    Compositions.Single(composition => string.Equals(composition.UnifiedViewKey, data.Value));

            }
            catch (Exception e)
            {
                
            }
        }
    }
}