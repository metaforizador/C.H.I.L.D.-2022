using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ConsumableSO)), CanEditMultipleObjects]
public class ConsumableSOEditor : Editor {

    public SerializedProperty
        nameProp, sprite, condition, consumableType,
        // Scanner
        identificationChance,

        // Battery
        shieldRecoveryPercentage, boostStaminaRecoverySpeed, boostAmmoRecoverySpeed, boostTimeInSeconds,

        // Comsat Link & Rig
        chanceToBeSuccessful,

        // Scrap
        chanceToTurnIntoToy, creditValue, craftValue,

        // Toy
        expToGain;

    void OnEnable() {
        // Setup the SerializedProperties
        nameProp = serializedObject.FindProperty("name");
        sprite = serializedObject.FindProperty("sprite");
        condition = serializedObject.FindProperty("condition");
        consumableType = serializedObject.FindProperty("consumableType");

        // Scanner
        identificationChance = serializedObject.FindProperty("identificationChance");

        // Battery
        shieldRecoveryPercentage = serializedObject.FindProperty("shieldRecoveryPercentage");
        boostStaminaRecoverySpeed = serializedObject.FindProperty("boostStaminaRecoverySpeed");
        boostAmmoRecoverySpeed = serializedObject.FindProperty("boostAmmoRecoverySpeed");
        boostTimeInSeconds = serializedObject.FindProperty("boostTimeInSeconds");

        // Comsat Link & Rig
        chanceToBeSuccessful = serializedObject.FindProperty("chanceToBeSuccessful");

        // Scrap
        chanceToTurnIntoToy = serializedObject.FindProperty("chanceToTurnIntoToy");
        creditValue = serializedObject.FindProperty("creditValue");
        craftValue = serializedObject.FindProperty("craftValue");

        // Toy
        expToGain = serializedObject.FindProperty("expToGain");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.PropertyField(nameProp);
        EditorGUILayout.PropertyField(sprite);
        EditorGUILayout.PropertyField(consumableType);
        EditorGUILayout.PropertyField(condition);

        ConsumableType type = (ConsumableType)consumableType.enumValueIndex;

        switch (type) {
            case ConsumableType.Scanner:
                EditorGUILayout.HelpBox(ConsumableSO.DESCRIPTION_SCANNER, MessageType.Info);
                EditorGUILayout.LabelField("Percentage chance to be successful", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(identificationChance);
                break;

            case ConsumableType.Battery:
                EditorGUILayout.HelpBox(ConsumableSO.DESCRIPTION_BATTERY, MessageType.Info);
                EditorGUILayout.LabelField("Percentage amount to recover", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(shieldRecoveryPercentage);
                EditorGUILayout.LabelField("Boost speed by multiplying with this value", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(boostStaminaRecoverySpeed);
                EditorGUILayout.PropertyField(boostAmmoRecoverySpeed);
                EditorGUILayout.LabelField("How long boost lasts", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(boostTimeInSeconds);
                break;

            case ConsumableType.ComsatLink:
                EditorGUILayout.HelpBox(ConsumableSO.DESCRIPTION_COMSAT_LINK, MessageType.Info);
                EditorGUILayout.LabelField("Percentage chance", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(chanceToBeSuccessful);
                break;

            case ConsumableType.Rig:
                EditorGUILayout.HelpBox(ConsumableSO.DESCRIPTION_RIG, MessageType.Info);
                EditorGUILayout.LabelField("Percentage chance", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(chanceToBeSuccessful);
                break;

            case ConsumableType.Scrap:
                EditorGUILayout.HelpBox(ConsumableSO.DESCRIPTION_SCRAP, MessageType.Info);
                EditorGUILayout.LabelField("Percentage chance", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(chanceToTurnIntoToy);
                EditorGUILayout.LabelField("Credit amounts are not yet decided", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(creditValue);
                EditorGUILayout.LabelField("Craft values are not yet decided", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(craftValue);
                break;

            case ConsumableType.Toy:
                EditorGUILayout.HelpBox(ConsumableSO.DESCRIPTION_TOY, MessageType.Info);
                EditorGUILayout.LabelField("Gives percentage amount of exp needed for next level", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(expToGain);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
