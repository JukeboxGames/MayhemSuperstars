using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    // Esta clase puede cambiar mediante se agregan mas cosas como animaciones, sprites, etc.
    [System.Serializable]
    private class EnemyStats
    {
        public string type;
        public int health;
        public int attack; 
        public int moveSpeed;
        public float attackSpeed; 
    }

    [System.Serializable]
    private class Enemies
    {
        public EnemyStats[] enemies;
    }


    protected int _health;
    protected int _attack;
    protected int _moveSpeed;
    protected float _attackSpeed; 
    private Enemies _enemyTypeList;
    protected virtual void Damage(int damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }

    protected void SetStats(string enemyType)
    {
        foreach(EnemyStats enemy in _enemyTypeList.enemies) {
            if(enemy.type == enemyType){
                _health = enemy.health;
                _attack = enemy.attack;
                _moveSpeed = enemy.moveSpeed;
                Debug.Log(enemy.attackSpeed);
                if(enemy.attackSpeed == 0) _attackSpeed = 0;
                else _attackSpeed = 1.0f/enemy.attackSpeed;
                break;
            }
        }
    }
    
    protected abstract void StartMoveCycle() ;
    protected abstract void StartAttackCycle();

    protected virtual void Initialize(string enemyType) {
        string jsonText = "";
        using (StreamReader sr = new("./Assets/Scripts/Enemy/EnemyStatsJson.json")){
            string line;
            while((line = sr.ReadLine()) != null) jsonText += line;
        }
        _enemyTypeList = JsonUtility.FromJson<Enemies>(jsonText);
        SetStats(enemyType);
        StartMoveCycle();
        StartAttackCycle(); 
    }
}
