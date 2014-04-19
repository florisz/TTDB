
###################################################################################################################################
#
#                                        FUNCTIONS
#
###################################################################################################################################

# uuidgen.exe replacement 
function uuidgen 
{ 
   $guid=[guid]::NewGuid().ToString('d') 
   return $guid
} 

function processComplexType
{
 param([String] $_ComplexType, [int] $x, [int] $_level) 

    #
    # Format output hierarchy
    #
    $tabs=""
    for ($tabCounter=0; $tabCounter -lt $_level; $tabCounter=$tabCounter+1)
    {
        $tabs=$tabs+"`t"
    }

    add-content -path $OutputFile -value ($tabs+'<'+$_ComplexType+'>')
    
    #
    # Do while processing this ComplexType
    #
    For ($x=$x; $x -lt $ComplexTypes.length; $x=$x+1)
    {
        if ($ComplexTypes[$x].contains("</xs:complexType>"))
        {
            break
        }
        else
        {
            if ($ComplexTypes[$x].contains("xs:element"))
            {
                $startPosition=$ComplexTypes[$x].IndexOf('name="')+6
                $endPosition=$ComplexTypes[$x].IndexOf('type="')
                $_name=$ComplexTypes[$x].Substring($startPosition,($endPosition-$startPosition)-2)
 #               write-host "name: " $_name
                
                $startPosition=$ComplexTypes[$x].IndexOf('type="')+6             
                $endPosition=$ComplexTypes[$x].IndexOf('" />')
                $_type=$ComplexTypes[$x].Substring($startPosition,($endPosition-$startPosition))
 #               write-host "type: " $_type
                                
                #
                # Simple element
                #
                if ($_name -ne $_type)
                {
                    if ($_name -eq "RegistrationId")
                    {
                        $uuidgen=uuidgen
                        add-content -path $OutputFile -value ($tabs+"`t"+'<'+$_name+'>'+$uuidgen+'</'+$_name+'>')
                    }
                    else
                    {
                        add-content -path $OutputFile -value ($tabs+"`t"+'<'+$_name+'>'+$_name+'</'+$_name+'>')
                    }
                }
                else
  		        
                #
 		        # Process this sub-ComplexType
 		        #
                {
 		       	processComplexType $_name ($x+3) ($_level+1)
        		add-content -path $OutputFile -value ($tabs+"`t"+'</'+$_name+'>')
                }
 
            }
        }
    }
}

###################################################################################################################################
#
#                                        MAIN PROGRAM
#
###################################################################################################################################

#
# Get inputfile
# Inputfile must be a XSD file containing the right definitions
#
$_File = Read-Host "Specify input XSD CaseFile: "
if(!($_File))
{
	$_File=$_File+".xsd"
	if(!($_File))
	{
		write-host "file "+$_File+" not found";
		return $false
	}
}


#
# Do some format processing on the outputfilename
#
$XMLCaseFile = split-path -leaf $_File
$XMLCaseFile=$XMLCaseFile.replace(".XSD","")
$XMLCaseFile=$XMLCaseFile.replace(".xsd","")
$XMLCaseFile=$XMLCaseFile.replace(".Xsd","")

$CSVInputFile = ".\ObjectModelsCSV\"+$XMLCaseFile+".csv"
    
#
# Check if input CSV file exists
#
if(!(test-path $CSVInputFile))
{
    write-host "Missing file: "$CSVInputfile
    return $false
}

$OutputFile = ".\ObjectModelsXML\"+"Automodellen"+".xml"

# 
# Clear file before continuing, suppress error message if the outputfile does not yet exist
#
clear-content -ErrorAction SilentlyContinue -path $OutputFile

#
# Default content for XML output file
#
add-content -path $OutputFile -value '<?xml version="1.0" encoding="utf-8"?>'
add-content -path $OutputFile -value '<!-- Generated using PowerShell -->'

#
# Now get the schema information this script will need to generate XML
#
$ComplexTypes = get-content $_File
For ($x=0; $x -lt $ComplexTypes.length; $x=$x+1)
{

    #
    # Look for the first complexTypes, skip the ones without a name
    #
    $level=0
    if ($ComplexTypes[$x].contains("<xs:complexType name="))
    {
        $startPosition=$ComplexTypes[$x].IndexOf('name="')+6
        $endPosition=$ComplexTypes[$x].IndexOf('">')
        $ComplexType=$ComplexTypes[$x].Substring($startPosition,($endPosition-$startPosition))
        
 
 	    #
 	    # Process this ComplexType
 	    #
        processComplexType $ComplexType $x $level
        add-content -path $OutputFile -value ('</'+$ComplexType+'>')
        break
    }        
}
#end-of-script
