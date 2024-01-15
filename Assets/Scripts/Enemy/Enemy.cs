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
    }

    [System.Serializable]
    private class Enemies
    {
        public EnemyStats[] enemies;
    }

    protected int _health;
    protected int _attack;
    protected int _moveSpeed;
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

    protected void setStats(string enemyType)
    {
        foreach(EnemyStats enemy in _enemyTypeList.enemies) {
            if(enemy.type == enemyType){
                _health = enemy.health;
                _attack = enemy.attack;
                _moveSpeed = enemy.moveSpeed;
                break;
            }
        }
    }
    
    protected abstract void StartMoveCycle() ;
    protected abstract void StartAttackCycle();

    protected virtual void Start() {
        string jsonText = "";
        using (StreamReader sr = new("./Assets/Scripts/Enemy/EnemyStatsJson.json")){
            string line;

            while((line = sr.ReadLine()) != null) {
                jsonText += line;
            }
        }
        _enemyTypeList = JsonUtility.FromJson<Enemies>(jsonText);
        StartMoveCycle();
        StartAttackCycle(); 
    }
}
