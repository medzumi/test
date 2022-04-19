﻿using System;
using UnityEngine;

namespace Utilities.SerializeReferencing
{
    public class SerializeTypesAttribute : PropertyAttribute
    {
        public readonly Type InheritType;

        public SerializeTypesAttribute(Type inheritType)
        {
            InheritType = inheritType;
        }
    }
}