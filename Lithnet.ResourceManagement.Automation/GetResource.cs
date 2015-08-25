﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Management.Automation;
using Microsoft.ResourceManagement.WebServices;
using System.Collections;
using Lithnet.ResourceManagement.Client;

namespace Lithnet.ResourceManagement.Automation
{
    [Cmdlet(VerbsCommon.Get, "Resource", DefaultParameterSetName="GetResourceByKey")]
    public class GetResource : PSCmdlet
    {
        [Parameter(ParameterSetName = "GetResource", ValueFromPipeline = true, Mandatory = true, Position = 1)]
        public object ID { get; set; }

        [Parameter(ParameterSetName = "GetResourceByKey", Mandatory = true, ValueFromPipeline = true, Position = 1)]
        [Parameter(ParameterSetName = "GetResourceByKeys", Mandatory = true, ValueFromPipeline = true, Position = 1)]
        public string ObjectType { get; set; }

        [Parameter(ParameterSetName = "GetResourceByKey", Mandatory = true, Position = 2)]
        public string AttributeName { get; set; }

        [Parameter(ParameterSetName = "GetResourceByKey", Mandatory = true, Position = 3)]
        public object AttributeValue { get; set; }

        [Parameter(ParameterSetName = "GetResourceByKeys", Mandatory = true, Position = 2)]
        public Hashtable AttributeValuePairs { get; set; }

        [Parameter(ParameterSetName = "GetResourceByKey", Mandatory = false, Position = 4)]
        [Parameter(ParameterSetName = "GetResourceByKeys", Mandatory = false, Position = 3)]
        [Parameter(ParameterSetName = "GetResource", Mandatory = false, Position = 2)]
        public string[] AttributesToGet { get; set; }
        
        protected override void ProcessRecord()
        {
            ResourceObject resource;

            UniqueIdentifier uniqueID = this.ID as UniqueIdentifier;

            if (uniqueID != null)
            {
                resource = RmcWrapper.Client.GetResource(uniqueID, this.AttributesToGet);

                if (resource == null)
                {
                    throw new ResourceNotFoundException();
                }

                this.WriteObject(new RmaObject(resource));
                return;
            }
            
            string stringID = this.ID as string;

            if (stringID != null)
            {
                resource = RmcWrapper.Client.GetResource(stringID, this.AttributesToGet);

                if (resource == null)
                {
                    throw new ResourceNotFoundException();
                }

                this.WriteObject(new RmaObject(resource));
            }

            Guid? guidID = this.ID as Guid?;

            if (guidID != null)
            {
                resource = RmcWrapper.Client.GetResource(guidID, this.AttributesToGet);

                if (resource == null)
                {
                    throw new ResourceNotFoundException();
                }

                this.WriteObject(new RmaObject(resource));
                return;
            }

            if (this.AttributeValuePairs != null)
            {
                resource = RmcWrapper.Client.GetResourceByKey(this.ObjectType, this.HashTableToDictionary(this.AttributeValuePairs), this.AttributesToGet);

                if (resource == null)
                {
                    throw new ResourceNotFoundException();
                }

                this.WriteObject(new RmaObject(resource));
                return;
            }
            else
            {
                resource = RmcWrapper.Client.GetResourceByKey(this.ObjectType, this.AttributeName, this.AttributeValue, this.AttributesToGet);

                if (resource == null)
                {
                    throw new ResourceNotFoundException();
                }

                this.WriteObject(new RmaObject(resource));
                return;
            }
        }

        private Dictionary<string, object> HashTableToDictionary(Hashtable table)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            foreach (DictionaryEntry entry in table)
            {
                dictionary.Add(entry.Key.ToString(), entry.Value);
            }

            return dictionary;
        }
    }
}
