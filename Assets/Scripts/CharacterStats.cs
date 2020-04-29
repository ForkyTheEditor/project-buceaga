using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CharacterStats : MonoBehaviour
{
    [SerializeField]
    private int _currentHealth;
    [SerializeField]
    private int _maxHealth;
    [SerializeField]
    private Teams _team;


    public int currentHealth { get { return _currentHealth; } }
    public int maxHealth { get { return _maxHealth; } }
    public Teams team { get { return _team; } }
}
