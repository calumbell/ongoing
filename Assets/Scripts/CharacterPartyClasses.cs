using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Skill
{
    Skirimish,
    Prowl,
}

public class Party
{
    CharacterSheet[] roster;
    int gambits;
}

public class CharacterSheet
{
    string name;
    int currentHealth;
    int maxHealth;
    int currentStress;
    int maxStress;

    // Perhaps replace this with a spritesheet to populate the animator with?
    GameObject prefab;

    Dictionary<Skill, int> skills;

    CharacterSheet(string _name, Dictionary<Skill, int> _skills)
    {
        name = _name;

        skills = new Dictionary<Skill, int>();

        if (_skills != null)
        {
            foreach (KeyValuePair<Skill, int> skill in _skills)
            {
                skills.Add(skill.Key, skill.Value);
            }
        }
    }

    public int RollStat(Skill _skill)
    {
        if (skills.ContainsKey(_skill))
        {
            int currentValue;
            int highestValue = 0;

            // roll d6's equal to skill rating and return the highest roll
            for (int i = 0; i < skills[_skill]; i++)
            {
                currentValue = Random.Range(1, 7);
                highestValue = currentValue > highestValue ? currentValue : highestValue;
            }

            return highestValue;
        }

        else
        {
            // roll 2d6 and return the lowest
            int firstValue = Random.Range(1, 7);
            int secondValue = Random.Range(1, 7);
            return firstValue < secondValue ? firstValue : secondValue;
        }

    }
}
