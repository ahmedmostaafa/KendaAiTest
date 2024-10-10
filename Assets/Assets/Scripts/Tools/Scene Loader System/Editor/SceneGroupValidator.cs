using System.Linq;
using KabreetGames.SceneManagement;
using Sirenix.OdinInspector.Editor.Validation;

[assembly: RegisterValidator(typeof(SceneGroupValidator))]
public class SceneGroupValidator : ValueValidator<SceneGroup>
{
    private const string AllZeroGuid = "00000000000000000000000000000000";

    protected override void Validate(ValidationResult result)
    {
        
        if(string.IsNullOrEmpty(Value.groupName))
        {
            result.AddError("Group name cannot be empty");
        }

        if (Value.scenes.Count == 0)
        {
            result.AddError("Group cannot be empty");
        }

        if (Value.scenes.Any(s => s.reference.Guid == AllZeroGuid))
        {
            result.AddError("Group cannot contain null references");
        }
    }
}
