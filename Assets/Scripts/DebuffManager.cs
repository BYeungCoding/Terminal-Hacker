using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI; // For UI elements if needed, though not used in this snippet

public class DebuffManager : MonoBehaviour
{
    [System.Serializable]
    public class DebuffDisplay
    {
        public DebuffType type; // The type of the debuff (FirewallRage, Slow, Curruption)
        public Image iconObject; // The UI object to display the icon (assign in the inspector)
    }

    public AngerMeter angerMeter;
    public CharacterMover characterMover;
    public List<DebuffDisplay> debuffDisplays = new List<DebuffDisplay>(); // List of debuff displays to show on the UI
    public HashSet<DebuffType> activeDebuffs = new HashSet<DebuffType>(); // Track active debuffs 

    public void ApplyDebuff(DebuffType type)
    {
        /*
         * This method applies a debuff to the character. It will update the UI and the anger meter accordingly.
         */
        if (activeDebuffs.Contains(type))
        {
            return; // Debuff already applied, no need to reapply
        }

        activeDebuffs.Add(type);
        UpdateIcons();

        if(type == DebuffType.FirewallRage)
        {
            // Special case for FirewallRage, you can add specific behavior here if needed
            if (angerMeter != null)
            {
                angerMeter.AppyDebuff(true); // Indicate character is debuffed
            }
        }

        if(type == DebuffType.Slow)
        {
            characterMover.SetMoveSpeed(2f);
        }
            
        else if (type == DebuffType.Curruption)
        {
            // Handle Curruption debuff logic here
            // Example: Apply some other game-specific behavior
        }

        //Add more later
    }    

    public void RemoveDebuff(DebuffType type)
    {
        /*
         * This method removes a debuff from the character. It will update the UI and the anger meter accordingly.
         */
        if (!activeDebuffs.Contains(type))
        {
            return; // Debuff not active, nothing to remove
        }

        activeDebuffs.Remove(type);
        UpdateIcons();

        if(type == DebuffType.FirewallRage)
        {
            if (angerMeter != null)
            {
                angerMeter.AppyDebuff(false); // Indicate character is no longer debuffed
            }
        }
        if (type == DebuffType.Slow)
        {
            characterMover.ResetMoveSpeed(); // back to normal
        }

        //Add more later
    }

    void UpdateIcons()
    {
        /*
         * This method updates the debuff icons based on the active debuffs.
         */
        foreach (var display in debuffDisplays)
        {
            if (activeDebuffs.Contains(display.type))
            {
                display.iconObject.gameObject.SetActive(true); // Show the icon for this debuff
            }
            else
            {
                display.iconObject.gameObject.SetActive(false); // Hide the icon if this debuff is not active
            }
        }
    }

    public bool HasDebuff(DebuffType type)
    {
        /*
         * This method checks if a specific debuff is currently active.
         */
        return activeDebuffs.Contains(type);
    }

}
