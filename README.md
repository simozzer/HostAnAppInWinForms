A friend of mine was finding it tricky to embed an external application within a c# WinForms application and getting resizing to work correctly. 

I can understand why this wsa tricky - there is a lot of conflicting advice available on the internet, and several examples that do not work at all.

Using Interop and the 'good old' Win32 API this is how I got it working with 'Notepad' embedded in a single form.
