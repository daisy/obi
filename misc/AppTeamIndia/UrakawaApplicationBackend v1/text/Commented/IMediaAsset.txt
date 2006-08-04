using System;
using System.Collections;
using System.Text;

namespace urakawaApplication
{
    public interface IMediaAsset
    {
        string getName();
        //returns the MediaAsset type, else use reflection 'instanceof'
        //this function should be virtual/abstract
        //ie, an abstract base class implementation of this interface would not implement this function
        Object getType();

        //[returns the MIME type string]
        //this function should be virtual/abstract
        //ie, an abstract base class implementation of this interface would not implement this function
        string getMediaType();

        double getSizeInBytes();

        //return value should, in the end, be some platform-accepted File object
        Object getFile();

        bool exists();
        bool canRead();
        bool canWrite();

        //this method should be "protected" not "public", but how to specify that in csharp?
        bool delete();

        Uri getURL();

        //will return FileReader not Object
        Object getFileReader();

        //performs integrity check on an asset, subtype dependent
        //this function should be virtual/abstract
        //ie, an abstract base class implementation of this interface would not implement this function
        bool validate();    

    }
}
