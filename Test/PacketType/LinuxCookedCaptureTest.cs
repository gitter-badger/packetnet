/*
This file is part of PacketDotNet

PacketDotNet is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

PacketDotNet is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with PacketDotNet.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using NUnit.Framework;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test.PacketType
{
    [TestFixture]
    public class LinuxCookedCaptureTest
    {
        private void VerifyPacket0(Packet p)
        {
            // expect an arp packet
            var arpPacket = p.Extract<ArpPacket>();
            Assert.IsNotNull(arpPacket, "Expected arpPacket to not be null");

            // validate some of the LinuxSSLPacket fields
            var l = (LinuxSllPacket) p;
            Assert.AreEqual(6, l.LinkLayerAddressLength, "Address length");
            Assert.AreEqual(1, l.LinkLayerAddressType);
            Assert.AreEqual(LinuxSllType.PacketSentToUs, l.Type);

            // validate some of the arp fields
            Assert.AreEqual("192.168.1.1",
                            arpPacket.SenderProtocolAddress.ToString(),
                            "Arp SenderProtocolAddress");

            Assert.AreEqual("192.168.1.102",
                            arpPacket.TargetProtocolAddress.ToString(),
                            "Arp TargetProtocolAddress");
        }

        private void VerifyPacket1(Packet p)
        {
            // expect a udp packet
            Assert.IsNotNull(p.Extract<UdpPacket>(), "expected a udp packet");
        }

        private void VerifyPacket2(Packet p)
        {
            // expecting a tcp packet
            Assert.IsNotNull(p.Extract<TcpPacket>(), "expected a tcp packet");
        }

        [Test]
        public void CookedCaptureTest()
        {
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "LinuxCookedCapture.pcap");
            dev.Open();

            RawCapture rawCapture;
            var packetIndex = 0;
            while ((rawCapture = dev.GetNextPacket()) != null)
            {
                var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);
                switch (packetIndex)
                {
                    case 0:
                    {
                        VerifyPacket0(p);
                        break;
                    }
                    case 1:
                    {
                        VerifyPacket1(p);
                        break;
                    }
                    case 2:
                    {
                        VerifyPacket2(p);
                        break;
                    }
                    default:
                    {
                        Assert.Fail("didn't expect to get to packetIndex " + packetIndex);
                        break;
                    }
                }

                packetIndex++;
            }

            dev.Close();
        }

        [Test]
        public void PrintString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "LinuxCookedCapture.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            var rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            Console.WriteLine("Parsing");
            var l = (LinuxSllPacket) p;

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(l.ToString());
        }

        [Test]
        public void PrintVerboseString()
        {
            Console.WriteLine("Loading the sample capture file");
            var dev = new CaptureFileReaderDevice(NUnitSetupClass.CaptureDirectory + "LinuxCookedCapture.pcap");
            dev.Open();
            Console.WriteLine("Reading packet data");
            var rawCapture = dev.GetNextPacket();
            var p = Packet.ParsePacket(rawCapture.GetLinkLayers(), rawCapture.Data);

            Console.WriteLine("Parsing");
            var l = (LinuxSllPacket) p;

            Console.WriteLine("Printing human readable string");
            Console.WriteLine(l.ToString(StringOutputType.Verbose));
        }
    }
}