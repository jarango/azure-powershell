// 
// Copyright (c) Microsoft and contributors.  All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// 
// See the License for the specific language governing permissions and
// limitations under the License.
// 

// Warning: This code was generated by a tool.
// 
// Changes to this file may cause incorrect behavior and will be lost if the
// code is regenerated.

using Microsoft.Azure.Commands.Compute.Automation.Models;
using Microsoft.Azure.Management.Compute;
using Microsoft.Azure.Management.Compute.Models;
using Microsoft.WindowsAzure.Commands.Utilities.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.Compute.Automation
{
    public partial class InvokeAzureComputeMethodCmdlet : ComputeAutomationBaseCmdlet
    {
        protected object CreateImageCreateOrUpdateDynamicParameters()
        {
            dynamicParameters = new RuntimeDefinedParameterDictionary();
            var pResourceGroupName = new RuntimeDefinedParameter();
            pResourceGroupName.Name = "ResourceGroupName";
            pResourceGroupName.ParameterType = typeof(string);
            pResourceGroupName.Attributes.Add(new ParameterAttribute
            {
                ParameterSetName = "InvokeByDynamicParameters",
                Position = 1,
                Mandatory = true
            });
            pResourceGroupName.Attributes.Add(new AllowNullAttribute());
            dynamicParameters.Add("ResourceGroupName", pResourceGroupName);

            var pImageName = new RuntimeDefinedParameter();
            pImageName.Name = "ImageName";
            pImageName.ParameterType = typeof(string);
            pImageName.Attributes.Add(new ParameterAttribute
            {
                ParameterSetName = "InvokeByDynamicParameters",
                Position = 2,
                Mandatory = true
            });
            pImageName.Attributes.Add(new AllowNullAttribute());
            dynamicParameters.Add("ImageName", pImageName);

            var pParameters = new RuntimeDefinedParameter();
            pParameters.Name = "Image";
            pParameters.ParameterType = typeof(Image);
            pParameters.Attributes.Add(new ParameterAttribute
            {
                ParameterSetName = "InvokeByDynamicParameters",
                Position = 3,
                Mandatory = true
            });
            pParameters.Attributes.Add(new AllowNullAttribute());
            dynamicParameters.Add("Image", pParameters);

            var pArgumentList = new RuntimeDefinedParameter();
            pArgumentList.Name = "ArgumentList";
            pArgumentList.ParameterType = typeof(object[]);
            pArgumentList.Attributes.Add(new ParameterAttribute
            {
                ParameterSetName = "InvokeByStaticParameters",
                Position = 4,
                Mandatory = true
            });
            pArgumentList.Attributes.Add(new AllowNullAttribute());
            dynamicParameters.Add("ArgumentList", pArgumentList);

            return dynamicParameters;
        }

        protected void ExecuteImageCreateOrUpdateMethod(object[] invokeMethodInputParameters)
        {
            string resourceGroupName = (string)ParseParameter(invokeMethodInputParameters[0]);
            string imageName = (string)ParseParameter(invokeMethodInputParameters[1]);
            Image parameters = (Image)ParseParameter(invokeMethodInputParameters[2]);

            var result = ImagesClient.CreateOrUpdate(resourceGroupName, imageName, parameters);
            WriteObject(result);
        }
    }

    public partial class NewAzureComputeArgumentListCmdlet : ComputeAutomationBaseCmdlet
    {
        protected PSArgument[] CreateImageCreateOrUpdateParameters()
        {
            string resourceGroupName = string.Empty;
            string imageName = string.Empty;
            Image parameters = new Image();

            return ConvertFromObjectsToArguments(
                 new string[] { "ResourceGroupName", "ImageName", "Parameters" },
                 new object[] { resourceGroupName, imageName, parameters });
        }
    }

    [Cmdlet(VerbsCommon.New, "AzureRmImage", DefaultParameterSetName = "DefaultParameter", SupportsShouldProcess = true)]
    [OutputType(typeof(PSImage))]
    public partial class NewAzureRmImage : ComputeAutomationBaseCmdlet
    {
        protected override void ProcessRecord()
        {
            ExecuteClientAction(() =>
            {
                if (ShouldProcess(this.ImageName, VerbsCommon.New))
                {
                    string resourceGroupName = this.ResourceGroupName;
                    string imageName = this.ImageName;
                    Image parameters = new Image();
                    ComputeAutomationAutoMapperProfile.Mapper.Map<PSImage, Image>(this.Image, parameters);

                    var result = ImagesClient.CreateOrUpdate(resourceGroupName, imageName, parameters);
                    var psObject = new PSImage();
                    ComputeAutomationAutoMapperProfile.Mapper.Map<Image, PSImage>(result, psObject);
                    WriteObject(psObject);
                }
            });
        }

        [Parameter(
            ParameterSetName = "DefaultParameter",
            Position = 1,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = false)]
        [AllowNull]
        [ResourceManager.Common.ArgumentCompleters.ResourceGroupCompleter()]
        public string ResourceGroupName { get; set; }

        [Parameter(
            ParameterSetName = "DefaultParameter",
            Position = 2,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = false)]
        [Alias("Name")]
        [AllowNull]
        public string ImageName { get; set; }

        [Parameter(
            ParameterSetName = "DefaultParameter",
            Position = 3,
            Mandatory = true,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = true)]
        [AllowNull]
        public PSImage Image { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Run cmdlet in the background")]
        public SwitchParameter AsJob { get; set; }
    }

    [Cmdlet(VerbsData.Update, "AzureRmImage", DefaultParameterSetName = "DefaultParameter", SupportsShouldProcess = true)]
    [OutputType(typeof(PSImage))]
    public partial class UpdateAzureRmImage : ComputeAutomationBaseCmdlet
    {
        public override void ExecuteCmdlet()
        {
            ExecuteClientAction(() =>
            {
                if (ShouldProcess(this.ImageName, VerbsData.Update))
                {
                    string resourceGroupName = this.ResourceGroupName;
                    string imageName = this.ImageName;
                    Image parameters = new Image();
                    ComputeAutomationAutoMapperProfile.Mapper.Map<PSImage, Image>(this.Image, parameters);

                    var result = ImagesClient.CreateOrUpdate(resourceGroupName, imageName, parameters);
                    var psObject = new PSImage();
                    ComputeAutomationAutoMapperProfile.Mapper.Map<Image, PSImage>(result, psObject);
                    WriteObject(psObject);
                }
            });
        }

        [Parameter(
            ParameterSetName = "DefaultParameter",
            Position = 1,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = false)]
        [AllowNull]
        [ResourceManager.Common.ArgumentCompleters.ResourceGroupCompleter()]
        public string ResourceGroupName { get; set; }

        [Parameter(
            ParameterSetName = "DefaultParameter",
            Position = 2,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = false)]
        [Alias("Name")]
        [AllowNull]
        public string ImageName { get; set; }

        [Parameter(
            ParameterSetName = "DefaultParameter",
            Position = 3,
            Mandatory = true,
            ValueFromPipelineByPropertyName = false,
            ValueFromPipeline = true)]
        [AllowNull]
        public PSImage Image { get; set; }
    }
}
