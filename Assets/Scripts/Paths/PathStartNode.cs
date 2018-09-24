using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent((typeof(PathNode)))]
public class PathStartNode : MonoBehaviour {
	[SerializeField] public int playerIndex;
}
