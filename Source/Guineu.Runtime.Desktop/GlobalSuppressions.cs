[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Scope = "member", Target = "Guineu.SECONDS..cctor()")]

// The Name of the desktop runtime is "guineu.runtime.dll" to avoid naming problems on Linux. When I used upper case characters
// in some cases it ended up on the Linux system in lower case naming. This caused any Guineu application to fail.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "guineu")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "runtime")]

// TODO: Deal with this later. Where can I put the dictionary file
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Guineu", Scope = "namespace", Target = "Guineu.Core")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Guineu", Scope = "namespace", Target = "Guineu")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Guineu", Scope = "namespace", Target = "Guineu.Data")]
