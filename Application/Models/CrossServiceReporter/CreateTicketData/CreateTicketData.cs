﻿using BocchiTracker.Config.Configs;
using BocchiTracker.Config;
using BocchiTracker.IssueInfoCollector;
using BocchiTracker.ServiceClientAdapters.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.CrossServiceReporter.CreateTicketData
{
    public class CreateTicketData : ICreateUnifiedTicketData<TicketData>
    {
        public TicketData? Create(ServiceDefinitions inService, IssueInfoBundle inBundle, ServiceConfig inConfig)
        {
            var ticket_data = new TicketData
            {
                TicketType      = new CreateTicketType().Create(inService, inBundle, inConfig),
                Summary         = new CreateSummary().Create(inService, inBundle, inConfig),
                Assignee        = new CreateAssignUser().Create(inService, inBundle, inConfig),
                Watcheres       = new CreateWatchUser().Create(inService, inBundle, inConfig),
                CustomFields    = new CreateCustomfields().Create(inService, inBundle, inConfig),
                Lables          = new CreateLabels().Create(inService, inBundle, inConfig),
                Priority        = new CreatePriority().Create(inService, inBundle, inConfig),
                Description     = new CreateDescription().Create(inService, inBundle, inConfig)
            };
            return ticket_data;
        }
    }
}
