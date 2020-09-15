/* 
 * Void data, designed to be used with the ScriptableObject event system.
 * It is just an empty struct, but even on a void event triggering, we need
 * to pass an argument.
 */

[System.Serializable] public struct Void { }