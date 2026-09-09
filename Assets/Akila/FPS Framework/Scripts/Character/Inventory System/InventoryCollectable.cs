using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Akila.FPSFramework
{
    [System.Serializable]
    public class InventoryCollectable
    {
        [SerializeField] int m_count = 1;
        [SerializeField] int m_limit = 1000;
        public InventoryCollectableIdentifier identifier;

        public int count
        {
            get
            {
                return m_count;
            }
            set
            {
                if (value > m_limit) m_count = m_limit;

                m_count = value;
            }
        }

        public void Add(int value = 1)
        {
            if (m_count < m_limit) m_count += value;
        }

        public void Remove(int value = 1)
        {
            if (m_count > 0) m_count -= value;
        }

        public void SetCount(int value)
        {
            count = value;
        }

        public void SetEmpty()
        {
            m_count = 0;
        }

        public bool IsEmpty()
        {
            return m_count <= 0;
        }

        public float GetLimit()
        {
            return m_limit;
        }

        public bool IsFull()
        {
            return m_count >= m_limit;
        }

        public InventoryCollectableIdentifier GetIdentifier()
        {
            return identifier;
        }

        public InventoryCollectable()
        {

        }
    }
}