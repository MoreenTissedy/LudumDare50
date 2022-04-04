using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class VisitorManager : MonoBehaviour
    {
        public Villager[] villagers;
        public Visitor[] visitors;

        private Visitor currentVisitor;

        public static VisitorManager instance;

        private void Awake()
        {
            instance = this;
        }

        public void Enter(Villager villager)
        {
            for (int i = 0; i < villagers.Length; i++)
            {
                if (villagers[i] == villager)
                {
                    visitors[i].Enter();
                    currentVisitor = visitors[i];
                }
            }
        }

        public void EnterDefault()
        {
            visitors[0].Enter();
            currentVisitor = visitors[0];
        }

        public void Exit()
        {
            currentVisitor.Exit();
        }
    }
}