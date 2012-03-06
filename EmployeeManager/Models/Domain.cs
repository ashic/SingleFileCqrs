using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EmployeeManager.Framework;

// ReSharper disable CheckNamespace
namespace EmployeeManager.Domain
// ReSharper restore CheckNamespace
{
    public class CreateEmployeeCommand
    {
        public Guid EmployeeId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class ChangeEmployeeNameCommand
    {
        public Guid EmployeeId { get; set; }
        public string Name { get; set; }
    }

    public class EmployeeCreatedEvent
    {
        public Guid EmployeeId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class EmployeeNameChangedEvent
    {
        public Guid EmployeeId { get; set; }
        public string Name { get; set; }
    }

    public class Employee : AggregateRoot
    {
        public Employee(Guid id, string name, string address)
            : base(id)
        {
            var @event = new EmployeeCreatedEvent { Address = address, EmployeeId = id, Name = name };
            Apply(@event);
        }

        protected void UpdateFrom(EmployeeCreatedEvent @event)
        {
            Id = @event.EmployeeId;
        }

        private Employee() { }

        public void ChangeName(string newName)
        {
            var @event = new EmployeeNameChangedEvent { EmployeeId = (Guid)this.Id, Name = newName };
            Apply(@event);
        }
    }

    public class EmployeeCommandExecutors
    {
        readonly IRepository _repo;

        public EmployeeCommandExecutors(IRepository repo)
        {
            _repo = repo;
        }

        public void Handle(CreateEmployeeCommand command)
        {
            var employee = new Employee(command.EmployeeId, command.Name, command.Address);

            _repo.Save(employee);
        }

        public void Handle(ChangeEmployeeNameCommand command)
        {
            var employee = _repo.GetById<Employee>(command.EmployeeId);
            employee.ChangeName(command.Name);

            _repo.Save(employee);
        }
    }
}