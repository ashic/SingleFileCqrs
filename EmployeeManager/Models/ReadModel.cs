using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EmployeeManager.Domain;
using Raven.Client;

// ReSharper disable CheckNamespace
namespace EmployeeManager.ReadModel
// ReSharper restore CheckNamespace
{
    public class EmployeeListItem
    {
        public string EmployeeId { get; set; }
        public string Name { get; set; }
    }

    public class EmployeeDetails
    {
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class EmployeeListItemDenormalizer
    {
        readonly IDocumentStore _db;

        public EmployeeListItemDenormalizer(IDocumentStore db)
        {
            _db = db;
        }

        public void Handle(EmployeeCreatedEvent @event)
        {
            var item = new EmployeeListItem { EmployeeId = @event.EmployeeId.ToString(), Name = @event.Name };
            PerformDbAction(x => x.Store(item));
        }

        public void Handle(EmployeeNameChangedEvent @event)
        {
            PerformDbAction(x =>
            {
                var employee = x.Query<EmployeeListItem>().Single(y => y.EmployeeId == @event.EmployeeId.ToString());
                employee.Name = @event.Name;
            });
        }

        void PerformDbAction(Action<IDocumentSession> action)
        {
            using (var session = _db.OpenSession())
            {
                action(session);
                session.SaveChanges();
            }
        }
    }

    public class EmployeeDetailsDenormalizer
    {
        readonly IDocumentStore _db;

        public EmployeeDetailsDenormalizer(IDocumentStore db)
        {
            _db = db;
        }

        public void Handle(EmployeeCreatedEvent @event)
        {
            var item = new EmployeeDetails { EmployeeId = @event.EmployeeId.ToString(), Name = @event.Name, Address = @event.Address };
            PerformDbAction(x => x.Store(item));
        }

        public void Handle(EmployeeNameChangedEvent @event)
        {
            PerformDbAction(x =>
            {
                var employee = x.Query<EmployeeDetails>().Single(y => y.EmployeeId == @event.EmployeeId.ToString());
                employee.Name = @event.Name;
            });
        }

        void PerformDbAction(Action<IDocumentSession> action)
        {
            using (var session = _db.OpenSession())
            {
                action(session);
                session.SaveChanges();
            }
        }
    }
}