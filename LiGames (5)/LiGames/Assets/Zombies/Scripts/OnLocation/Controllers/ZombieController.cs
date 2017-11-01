using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : Person
{
    public int visibleDistanceInCells = 10;
    public int randomDistanceInCells = 2;
    float visibleDistance;
    float randomDistance;
    float radius;
    List<PersonActionController> personsNear;
    Field field;
    float usualSpeed;
    public float randomWalkingSpeed = 0.5f;
    public float damage = 1f;
    public bool IsAttacking { get; private set; }
    public float attackingTime = .5f;

    public bool StepIsOver { get; private set; }
    IEnumerator decision;

    bool IsAnybodyNear
    {
        get
        {
            personsNear = new List<PersonActionController>();
            foreach (PersonActionController person in GameObject.FindObjectsOfType<PersonActionController>())
                if ((person.transform.position - transform.position).magnitude <= visibleDistance)
                    personsNear.Add(person);
            return personsNear.Count > 0;
        }
    }

    new void Awake()
    {
        base.Awake();
        field = GameObject.FindObjectOfType<Field>();
        usualSpeed = navigationAgent.speed;
        IsAttacking = false;
    }
    new void Start()
    {
        base.Start();
        visibleDistance = visibleDistanceInCells * field.CellSize;
        randomDistance = randomDistanceInCells * field.CellSize;
        radius = radiusInCells * field.CellSize;
        healthBar.gameObject.SetActive(false);
    }

    #region API
    public void MakeDecision()
    {
        StepIsOver = false;
        if (IsAnybodyNear)
        {
            PersonActionController nearestPerson = GetNearestPerson();
            List<FieldCell> nearestCells;
            FieldCell nearestToPersonCell = null;
            navigationAgent.speed = usualSpeed;

            if (IsHumanVeryClose(nearestPerson))
            {
                decision = Attack(nearestPerson);
                return;
            }
            else
            if (nearestPerson && nearestPerson.Alive && (transform.position - nearestPerson.transform.position).magnitude <= radius) // in radius
            {
                nearestCells = field.NearestCells(nearestPerson, field.CellSize);
                if (nearestCells.Count > 0)
                {
                    nearestToPersonCell = nearestCells[0];

                    foreach (FieldCell cell in nearestCells)
                        if ((cell.transform.position - transform.position).magnitude <
                            (nearestToPersonCell.transform.position - transform.position).magnitude)
                            nearestToPersonCell = cell;

                    field.StakeOutCell(nearestToPersonCell);
                    decision = MoveTowardsPerson(nearestToPersonCell, nearestPerson);
                    return;
                }
            }
            nearestCells = field.NearestCells(GetComponent<Person>(), radius);
            if (nearestCells.Count > 0)
            {
                nearestToPersonCell = nearestCells[0];

                foreach (FieldCell cell in nearestCells)
                    if (nearestPerson && nearestPerson.Alive &&(cell.transform.position - nearestPerson.transform.position).magnitude
                        < (nearestToPersonCell.transform.position - nearestPerson.transform.position).magnitude)
                        nearestToPersonCell = cell;

                field.StakeOutCell(nearestToPersonCell);
                decision = MoveTowardsPerson(nearestToPersonCell, nearestPerson);
                return;
            }
        }
        else
        {
            navigationAgent.speed = randomWalkingSpeed;

            List<FieldCell> randomArea = field.NearestCells(GetComponent<Person>(), randomDistance);
            if(randomArea.Count == 0)
                return;
            System.Random r = new System.Random();
            FieldCell randomCell = randomArea[r.Next(randomArea.Count)];
            field.StakeOutCell(randomCell);

            decision = MoveTowards(randomCell);
        }
    }
    public override IEnumerator ReceiveDamage(float damage)
    {
        CurrentHealth -= damage;
        if (Alive)
        {
            yield return new WaitForSeconds(receivingDamageTime);
            bloodSplatter.Play();
        }
    }
    public void DoDecision()
    {
        StartCoroutine(Step());
    }
    public int MaxShotsCanReceive(float damage)
    {
        int count = 0;
        float hlth = CurrentHealth;
        do
            ++count;
        while ((hlth -= damage) > 0.0f);
        return count;
    }
    #endregion

    IEnumerator Attack(PersonActionController nearestPerson)
    {
        if (nearestPerson)
        {
            yield return LookAtCell(field.NearestCell(nearestPerson.transform.position));

            Animator.SetFloat("Speed_f", 0.6f);
            yield return new WaitForSeconds(attackingTime / 2.0f);
            if (nearestPerson && nearestPerson.Alive)
                yield return nearestPerson.ReceiveDamage(damage);
            yield return new WaitForSeconds(attackingTime / 2.0f);
            Animator.SetFloat("Speed_f", 0.0f);
        }
    }
    IEnumerator MoveTowardsPerson(FieldCell cell, PersonActionController person)
    {
        yield return MoveTowards(cell);
        if (person && IsHumanVeryClose(person))
            yield return Attack(person);
    }
    IEnumerator Step()
    {
        yield return decision;
        StepIsOver = true;
    }
    PersonActionController GetNearestPerson()
    {
        PersonActionController nearest = null;
        foreach (PersonActionController person in GameObject.FindObjectsOfType<PersonActionController>())
        {
            if (!person.Alive)
                continue;
            if (nearest == null || (person.transform.position - transform.position).magnitude < (nearest.transform.position - transform.position).magnitude)
                nearest = person;
        }
        return nearest;
    }
    bool IsHumanVeryClose(PersonActionController nearestPerson)
    {
        return nearestPerson && nearestPerson.Alive && (transform.position - nearestPerson.transform.position).magnitude <= field.CellSize + Manager.GameInfo.radiusError + field.CellSize / 10.0f;
    }
    protected override IEnumerator Die()
    {
        Alive = false;
        Manager.RemovePersonFromField(GetComponent<Person>());
        Animator.SetBool("Dead_b", true);
        HideBar();
        yield return base.Die();
    }
}
