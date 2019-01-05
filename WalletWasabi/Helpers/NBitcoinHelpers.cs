﻿using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WalletWasabi.Helpers
{
	public static class NBitcoinHelpers
	{
		public static string HashOutpoints(IEnumerable<OutPoint> outPoints)
		{
			var sb = new StringBuilder();
			foreach (OutPoint input in outPoints.OrderBy(x => x.Hash.ToString()).ThenBy(x => x.N))
			{
				sb.Append(ByteHelpers.ToHex(input.ToBytes()));
			}

			return HashHelpers.GenerateSha256Hash(sb.ToString());
		}

		public static BitcoinAddress ParseBitcoinAddress(string address)
		{
			try
			{
				return BitcoinAddress.Create(address, Network.RegTest);
			}
			catch (FormatException)
			{
				try
				{
					return BitcoinAddress.Create(address, Network.TestNet);
				}
				catch (FormatException)
				{
					return BitcoinAddress.Create(address, Network.Main);
				}
			}
		}

		//public static Money TakeAReasonableFee(Money outputValue)
		//{
		//	Money fee = Money.Coins(0.002m);
		//	Money ret = outputValue - fee;
		//	var half = outputValue / 2;
		//	if (ret <= half)
		//	{
		//		return half;
		//	}

		//	return ret;
		//}

		//public static Money TakeAReasonableFee(Money outputValue)
		//{
		//	// sanity check
		//	if (outputValue < Money.Coins(0.00001m))
		//	{
		//		return outputValue;
		//	}

		//	Money fee = Money.Coins(0.002m);
		//	var remaining = Money.Zero;

		//	for (int i = 0; i < 100; i++)
		//	{
		//		remaining = outputValue - fee;
		//		if (remaining > Money.Coins(0.00001m))
		//		{
		//			break;
		//		}
		//		fee = fee.Percentange(50);
		//	}

		//	return remaining <= 0 ? outputValue : remaining;
		//}

		public static Money TakeAReasonableFee(Money outputValue)
		{
			// sanity check
			var sanity = Money.Coins(0.00001m);
			if (outputValue <= sanity)
			{
				return outputValue;
			}

			Money fee = Money.Coins(0.002m);
			var remaining = Money.Zero;

			while (true)
			{
				remaining = outputValue - fee;
				if (remaining > sanity)
				{
					break;
				}
				fee = fee.Percentange(50);
			}

			return remaining <= 0 ? outputValue : remaining;
		}

		public static int CalculateVsizeAssumeSegwit(int inNum, int outNum)
		{
			var origTxSize = inNum * Constants.P2pkhInputSizeInBytes + outNum * Constants.OutputSizeInBytes + 10;
			var newTxSize = inNum * Constants.P2wpkhInputSizeInBytes + outNum * Constants.OutputSizeInBytes + 10; // BEWARE: This assumes segwit only inputs!
			var vSize = (int)Math.Ceiling(((3 * newTxSize) + origTxSize) / 4m);
			return vSize;
		}
	}
}
