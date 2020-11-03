﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using MigrationTools.DataContracts;
using MigrationTools.Enrichers;

namespace MigrationTools.Endpoints
{
    public class InMemoryWorkItemEndpoint : WorkItemEndpoint
    {
        private List<WorkItemData2> _innerList;
        private InMemoryWorkItemEndpointOptions _Options;

        public InMemoryWorkItemEndpoint(EndpointEnricherContainer endpointEnrichers, IServiceProvider services, ITelemetryLogger telemetry, ILogger<WorkItemEndpoint> logger) : base(endpointEnrichers, services, telemetry, logger)
        {
            _innerList = new List<WorkItemData2>();
        }

        public override int Count => _innerList.Count;
        public override EndpointDirection Direction => _Options.Direction;
        public override IEnumerable<IWorkItemProcessorSourceEnricher> SourceEnrichers => throw new NotImplementedException();
        public override IEnumerable<IWorkItemProcessorTargetEnricher> TargetEnrichers => throw new NotImplementedException();

        public override void Configure(IEndpointOptions options)
        {
            _Options = (InMemoryWorkItemEndpointOptions)options;
        }

        public WorkItemData2 CreateNewFrom(WorkItemData2 source)
        {
            _innerList.Add(source);
            return source;
        }

        public override void Filter(IEnumerable<WorkItemData2> workItems)
        {
            var ids = (from x in workItems select x.Id);
            _innerList = (from x in _innerList
                          where !ids.Contains(x.Id)
                          select x).ToList();
        }

        public override IEnumerable<WorkItemData2> GetWorkItems()
        {
            return _innerList;
        }

        public override IEnumerable<WorkItemData2> GetWorkItems(IWorkItemQuery query)
        {
            return GetWorkItems();
        }

        public override void PersistWorkItem(WorkItemData2 source)
        {
            var found = (from x in _innerList where x.Id == source.Id select x).SingleOrDefault();
            if (found is null)
            {
                found = CreateNewFrom(source);
            }
            foreach (IWorkItemProcessorTargetEnricher enricher in TargetEnrichers)
            {
                enricher.PersistFromWorkItem(source);
            }
            UpdateWorkItemFrom(found, source);
        }

        private void UpdateWorkItemFrom(WorkItemData2 source, WorkItemData2 target)
        {
            _innerList.Remove(source);
            _innerList.Add(target);
        }
    }
}