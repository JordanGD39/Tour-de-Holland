using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelection : MonoBehaviour
{
    public enum CharacterSelection { MOLEN, TULP, KAASBLOK, RONDEKAAS, KLOMP, PATAT, VIS, OLIEBOL };
    public List<CharacterSelection> characterSelections = new List<CharacterSelection>();
    
}
