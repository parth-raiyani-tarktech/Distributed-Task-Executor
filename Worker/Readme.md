# Run multiple instances of the worker

```sh
dotnet Worker.dll --name Worker1 --port 5000
```

For every execution, make sure to use a different port and name.

The application registers itself to task executor on startup