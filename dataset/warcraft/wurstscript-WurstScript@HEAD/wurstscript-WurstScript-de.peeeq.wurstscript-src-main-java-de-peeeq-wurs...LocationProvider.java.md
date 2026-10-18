---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstio/jassinterpreter/providers/LocationProvider.java"
ref: HEAD
---

package de.peeeq.wurstio.jassinterpreter.providers;

import de.peeeq.wurstio.jassinterpreter.mocks.LocationMock;
import de.peeeq.wurstscript.intermediatelang.ILconstReal;
import de.peeeq.wurstscript.intermediatelang.IlConstHandle;
import de.peeeq.wurstscript.intermediatelang.interpreter.AbstractInterpreter;

public class LocationProvider extends Provider {

    public LocationProvider(AbstractInterpreter interpreter) {
        super(interpreter);
    }

    public IlConstHandle Location(ILconstReal x, ILconstReal y) {
        return new IlConstHandle(NameProvider.getRandomName("location"), new LocationMock(x, y));
    }

    public void RemoveLocation(IlConstHandle location) {
    }

    public void MoveLocation(IlConstHandle location, ILconstReal x, ILconstReal y) {
        LocationMock locationMock = (LocationMock) location.getObj();
        locationMock.move(x, y);
    }

    public ILconstReal GetLocationX(IlConstHandle location) {
        LocationMock locationMock = (LocationMock) location.getObj();
        return locationMock.x;
    }

    public ILconstReal GetLocationY(IlConstHandle location) {
        LocationMock locationMock = (LocationMock) location.getObj();
        return locationMock.y;
    }
}
