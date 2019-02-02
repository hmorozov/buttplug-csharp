﻿// <copyright file="HidSharpDeviceImpl.cs" company="Nonpolynomial Labs LLC">
// Buttplug C# Source Code File - Visit https://buttplug.io for more info about the project.
// Copyright (c) Nonpolynomial Labs LLC. All rights reserved.
// Licensed under the BSD 3-Clause license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using Buttplug.Core;
using Buttplug.Core.Logging;
using Buttplug.Core.Messages;
using Buttplug.Devices;
using HidSharp;

namespace Buttplug.Server.Managers.HidSharpManager
{
    public class HidSharpDeviceImpl : ButtplugDeviceImpl
    {
        private HidDevice _device;
        private HidStream _stream;

        public override bool Connected => _stream == null;

        public HidSharpDeviceImpl(IButtplugLogManager aLogManager, HidDevice aDevice)
            : base(aLogManager)
        {
            _device = aDevice;
            aDevice.TryOpen(out _stream);
            Name = _device.GetProductName();
        }

        public override void Disconnect()
        {
            _stream.Close();
            _stream = null;
            _device = null;
        }

        public override async Task<ButtplugMessage> WriteValueAsync(uint aMsgId, byte[] aValue, bool aWriteWithResponse, CancellationToken aToken)
        {
            await _stream.WriteAsync(aValue, 0, aValue.Length, aToken).ConfigureAwait(false);
            return new Ok(aMsgId);
        }

        public override async Task<ButtplugMessage> WriteValueAsync(uint aMsgId, string aEndpointName, byte[] aValue, bool aWriteWithResponse,
            CancellationToken aToken)
        {
            if (aEndpointName != Endpoints.Tx)
            {
                throw new ButtplugDeviceException(BpLogger, "UwpHidDevice doesn't support any write endpoint except the default.", aMsgId);
            }

            return await WriteValueAsync(aMsgId, aValue, aWriteWithResponse, aToken).ConfigureAwait(false);
        }

        public override Task<(ButtplugMessage, byte[])> ReadValueAsync(uint aMsgId, CancellationToken aToken)
        {
            throw new NotImplementedException();
        }

        public override Task<(ButtplugMessage, byte[])> ReadValueAsync(uint aMsgId, string aEndpointName, CancellationToken aToken)
        {
            throw new NotImplementedException();
        }

        public override Task SubscribeToUpdatesAsync()
        {
            throw new NotImplementedException();
        }

        public override Task SubscribeToUpdatesAsync(string aEndpointName)
        {
            throw new NotImplementedException();
        }
    }
}