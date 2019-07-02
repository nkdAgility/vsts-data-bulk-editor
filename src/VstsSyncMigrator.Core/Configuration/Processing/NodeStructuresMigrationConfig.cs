﻿using System;
using System.Collections.Generic;

namespace VstsSyncMigrator.Engine.Configuration.Processing
{
    public class NodeStructuresMigrationConfig : ITfsProcessingConfig
    {
        public bool PrefixProjectToNodes { get; set; }
        /// <inheritdoc />
        public bool Enabled { get; set; }
        public string[] BasePaths { get; set; }
        /// <inheritdoc />
        public Type Processor
        {
            get { return typeof(NodeStructuresMigrationContext); }
        }                         
        /// <inheritdoc />
        public bool IsProcessorCompatible(IReadOnlyList<ITfsProcessingConfig> otherProcessors)
        {
            return true;
        }
    }
}