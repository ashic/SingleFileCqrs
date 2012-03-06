using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EmployeeManager.Domain;
using EmployeeManager.Framework;
using EmployeeManager.ReadModel;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Database.Server;

namespace EmployeeManager.Code
{
    public class Configuration
    {
        public static ICommandService CommandService { get; private set; }
        public static IRepository DomainRepository { get; private set; }
        public static IDocumentStore DocumentStore { get; private set; }

        private static MessageBus _bus;

        public static void Boot()
        {
            InitializeBus();
            InitializeRepository();
            RegisterHandlers();
        }

        private static void InitializeBus()
        {
            _bus = new MessageBus();
            CommandService = _bus;
        }

        private static void InitializeRepository()
        {
            var db = new EmbeddableDocumentStore()
            {
                UseEmbeddedHttpServer = true
            };

            NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8081);
            db.Configuration.Port = 8081;

            db.Initialize();

            DocumentStore = db;

            var eventStore = new RavenDbEventStore(db, _bus);
            var repository = new DomainRepository(eventStore);

            DomainRepository = repository;
        }

        private static void RegisterHandlers()
        {
            var employeeHandlers = new EmployeeCommandExecutors(DomainRepository);
            _bus.RegisterHandler<CreateEmployeeCommand>(employeeHandlers.Handle);
            _bus.RegisterHandler<ChangeEmployeeNameCommand>(employeeHandlers.Handle);


            var listDenormalizer = new EmployeeListItemDenormalizer(DocumentStore);
            _bus.RegisterHandler<EmployeeCreatedEvent>(listDenormalizer.Handle);
            _bus.RegisterHandler<EmployeeNameChangedEvent>(listDenormalizer.Handle);

            var detailsDenormalizer = new EmployeeDetailsDenormalizer(DocumentStore);
            _bus.RegisterHandler<EmployeeCreatedEvent>(detailsDenormalizer.Handle);
            _bus.RegisterHandler<EmployeeNameChangedEvent>(detailsDenormalizer.Handle);
        }
    }
}