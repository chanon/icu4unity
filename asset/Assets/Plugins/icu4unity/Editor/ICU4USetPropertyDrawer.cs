// ICU4U - modified class name, modified for multi line string
// Copyright (c) 2014 Luminary LLC
// Licensed under The MIT License (See LICENSE for full text)
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Reflection;

[CustomPropertyDrawer(typeof(ICU4USetPropertyAttribute))]
public class SetPropertyDrawer : PropertyDrawer {

	private const float Height = 60f;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		// Rely on the default inspector GUI
		//EditorGUI.BeginChangeCheck();
		//EditorGUI.PropertyField(position, property, label);

		Rect rc = new Rect(position.x, position.y, position.width, Height);
		EditorGUI.BeginProperty(rc, label, property);

		// add word wrap to style.
		GUIStyle style = new GUIStyle(EditorStyles.textField);
		style.wordWrap = true;

		EditorGUI.BeginChangeCheck();
		string input = EditorGUI.TextArea(rc, property.stringValue);

		// Update only when necessary
		ICU4USetPropertyAttribute setProperty = attribute as ICU4USetPropertyAttribute;
		if (EditorGUI.EndChangeCheck()) {
			// When a SerializedProperty is modified the actual field does not have the current value set (i.e.  
			// FieldInfo.GetValue() will return the prior value that was set) until after this OnGUI call has completed. 
			// Therefore, we need to mark this property as dirty, so that it can be updated with a subsequent OnGUI event 
			// (e.g. Repaint)
			property.stringValue = input;
			setProperty.IsDirty = true;
		}
		else if (setProperty.IsDirty) {
			// The propertyPath may reference something that is a child field of a field on this Object, so it is necessary
			// to find which object is the actual parent before attempting to set the property with the current value.
			object parent = GetParentObjectOfProperty(property.propertyPath, property.serializedObject.targetObject);
			Type type = parent.GetType();
			PropertyInfo pi = type.GetProperty(setProperty.Name);
			if (pi == null) {
				Debug.LogError("Invalid property name: " + setProperty.Name + "\nCheck your [SetProperty] attribute");
			}
			else {
				// Use FieldInfo instead of the SerializedProperty accessors as we'd have to deal with every 
				// SerializedPropertyType and use the correct accessor
				pi.SetValue(parent, fieldInfo.GetValue(parent), null);
			}
			setProperty.IsDirty = false;
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		return Height;
	}

	private object GetParentObjectOfProperty(string path, object obj) {
		string[] fields = path.Split('.');

		// We've finally arrived at the final object that contains the property
		if (fields.Length == 1) {
			return obj;
		}

		// We may have to walk public or private fields along the chain to finding our container object, so we have to allow for both
		FieldInfo fi = obj.GetType().GetField(fields[0], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		obj = fi.GetValue(obj);

		// Keep searching for our object that contains the property
		return GetParentObjectOfProperty(string.Join(".", fields, 1, fields.Length - 1), obj);
	}
}