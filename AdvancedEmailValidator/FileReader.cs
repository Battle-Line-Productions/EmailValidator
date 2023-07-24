#region Copyright
// ---------------------------------------------------------------------------
// Copyright (c) 2023 BattleLine Productions LLC. All rights reserved.
// 
// Licensed under the BattleLine Productions LLC license agreement.
// See LICENSE file in the project root for full license information.
// 
// Author: Michael Cavanaugh
// Company: BattleLine Productions LLC
// Date: 07/23/2023
// Project: Frontline CRM
// File: FileReader.cs
// ---------------------------------------------------------------------------
#endregion

using System.Diagnostics.CodeAnalysis;
using System.IO;
using AdvancedEmailValidator.Interfaces;
using System.Threading.Tasks;

namespace AdvancedEmailValidator;

[ExcludeFromCodeCoverage]
public class FileReader : IFileReader
{
    public bool Exists(string path)
    {
        return File.Exists(path);
    }

    public async Task<string[]> ReadAllLinesAsync(string path)
    {
        return await File.ReadAllLinesAsync(path);
    }
}