using System;
using KabreetGames.SceneReferences.Editor;
using Sirenix.OdinInspector.Editor.Validation;
using UnityEngine;
using Object = UnityEngine.Object;


[assembly: RegisterValidator(typeof(SceneRefAttributeOdinValidator<,>))]

namespace KabreetGames.SceneReferences.Editor
{
    public class SceneRefAttributeOdinValidator<TA, T> : AttributeValidator<TA, T>
        where T : class where TA : SceneReferenceAttribute
    {
        protected override void Validate(ValidationResult result)
        {
            if (!Attribute.Flags.HasFlag(Flag.Optional) && !IsValid(ValueEntry.SmartValue))
            {
                var error = Property.NiceName + " is required";
                result.AddError($"{error}");
                return;
            }

            if (Attribute.Location == ReferencesLocation.Anywhere) return;

            var gameObject1 = result.Setup.Root as GameObject;
            if (gameObject1 == null)
            {
                var root = result.Setup.Root as Component;
                if (root != null)
                    gameObject1 = root.gameObject;
            }

            var gameObject2 = (object)ValueEntry.SmartValue as GameObject;
            if (gameObject2 == null)
            {
                var smartValue = (object)ValueEntry.SmartValue as Component;
                if (smartValue != null)
                    gameObject2 = smartValue.gameObject;
            }

            if (gameObject1 == null || gameObject2 == null)
            {
                result.ResultType = ValidationResultType.IgnoreResult;
                return;
            }

            switch (Attribute.Location)
            {
                case ReferencesLocation.Self when gameObject1 == gameObject2:
                    result.ResultType = ValidationResultType.Valid;
                    break;
                case ReferencesLocation.Self:
                    result.ResultType = ValidationResultType.Error;
                    result.Message = gameObject2.name + " must be the same object " + gameObject1.name;
                    break;
                case ReferencesLocation.Child when gameObject2.transform.IsChildOf(gameObject1.transform) && gameObject1 != gameObject2:
                    result.ResultType = ValidationResultType.Valid;
                    break;
                case ReferencesLocation.Child:
                    result.ResultType = ValidationResultType.Error;
                    result.Message = gameObject2.name + " must be a only child of " + gameObject1.name;
                    break;
                case ReferencesLocation.SelfOrChild when gameObject2.transform.IsChildOf(gameObject1.transform):
                    result.ResultType = ValidationResultType.Valid;
                    break;
                case ReferencesLocation.SelfOrChild:
                    result.ResultType = ValidationResultType.Error;
                    result.Message = gameObject2.name + " must be a child or the same of " + gameObject1.name;
                    break;
                case ReferencesLocation.Parent when gameObject1.transform.IsChildOf(gameObject2.transform) && gameObject1 != gameObject2:
                    result.ResultType = ValidationResultType.Valid;
                    break;
                case ReferencesLocation.Parent:
                    result.ResultType = ValidationResultType.Error;
                    result.Message = gameObject2.name + " must be only a Parent of " + gameObject1.name;
                    break;
                case ReferencesLocation.SelfOrParent when gameObject1.transform.IsChildOf(gameObject2.transform):
                    result.ResultType = ValidationResultType.Valid;
                    break;
                case ReferencesLocation.SelfOrParent:
                    result.ResultType = ValidationResultType.Error;
                    result.Message = gameObject2.name + " must be a Parent of " + gameObject1.name;
                    break;
                case ReferencesLocation.Anywhere:
                    result.ResultType = ValidationResultType.IgnoreResult;
                    break;
                case ReferencesLocation.Scene:
                    result.ResultType = ValidationResultType.IgnoreResult;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static bool IsValid(T memberValue) => memberValue != null &&
                                                      ((object)memberValue is not string ||
                                                       !string.IsNullOrEmpty((object)memberValue as string)) &&
                                                      ((object)((object)memberValue as Object) == null ||
                                                       !((object)memberValue as Object == null));
    }
}