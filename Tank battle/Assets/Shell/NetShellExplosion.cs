using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyWork.Network.Tanks
{
    public class NetShellExplosion : MonoBehaviour
    {
        public LayerMask TankMask;//碰撞层
        public ParticleSystem ExplosionParticles;//爆炸粒子效果
        public AudioSource ExplosionAudio;//音频
        public float MaxDamage = 100f;//最大伤害值
        public float ExplosionForce = 1000f;//爆炸推力
        public float MaxLifeTime = 2f;//没有任何事件发生后的生存周期
        public float ExplosionRadius = 5f;//爆炸范围

        // Start is called before the first frame update
        void Start()
        {
            Destroy(gameObject, MaxLifeTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            //在TankMask层中，以transform.position为球心，ExplosionRadius为半径的球体内所有trigger
            Collider[] colliders = Physics.OverlapSphere(transform.position, ExplosionRadius, TankMask);
            for (int i = 0; i < colliders.Length; i++)
            {
                Rigidbody targetRigidboy = colliders[i].GetComponent<Rigidbody>();
                if (!targetRigidboy)
                    continue;
                targetRigidboy.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius);

                NetTankHealth targetHealth = targetRigidboy.GetComponent<NetTankHealth>();
                if (!targetRigidboy)
                    continue;
                float damage = CalculateDamage(targetRigidboy.position);
                targetHealth.TakeDamage(damage);
            }

            ExplosionParticles.transform.parent = null;
            ExplosionParticles.Play();
            ExplosionAudio.Play();

            Destroy(ExplosionParticles.gameObject, ExplosionParticles.main.duration);
            Destroy(gameObject);
        }

        float CalculateDamage(Vector3 targetPosition)
        {
            //获取爆炸点到目标点的向量
            Vector3 explosion2Target = targetPosition - transform.position;
            //向量大小
            float explosionDistance = explosion2Target.magnitude;
            //向量相对距离，值在0-1之间，计算方法：（爆炸半径-爆炸点到目标点距离）/爆炸半径
            float relativeDistance = (ExplosionRadius - explosionDistance) / ExplosionRadius;

            float damage = relativeDistance * MaxDamage;

            //用来确保伤害不为负值
            //可能存在负值情况就是，当tank的锚点在爆炸半径之外，而实际上，overlapSphere是有覆盖到的tank
            //简单来说，胶囊体，前面半截在爆炸半径内，所以overlapSphere有覆盖到，而实际上，计算伤害的时候，根据tank
            //的锚点，而实际上锚点在爆炸半径意外，因此，就会存在负值，情况，这种情况就把他设为0f
            damage = Mathf.Max(0f, damage);

            return damage;
        }

    }
}