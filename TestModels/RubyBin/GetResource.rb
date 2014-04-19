# Ruby script
#
#	Put the SimpleModel specification to the TimeLine Server
#
#	Includes
#
require 'rubygems'
require 'rexml/document'
require 'rest-open-uri'

#
#	global declarations 
#
ITSHOST = 'http://localhost:8080/its/'
FROM = "floris.zwarteveen@luminis.nl"
def ObjectModelUri(name)
	URI(ITSHOST + 'specifications/objectmodels/' + @name)
end
def CaseFileSpecUri(objectmodelname, name)
	URI(ITSHOST + 'specifications/casefiles/' + @objectmodelname + '/' + @name)
end
def RepresentationSpecUri(objectmodelname, casefilespecname, name)
	URI(ITSHOST + 'representations/' + @objectmodelname + '/' + @casefilespecname + '/' + @name)
end
def RuleSetUri(objectmodelname, casefilespecname, name)
	URI(ITSHOST + 'rules/' + @objectmodelname + '/' + @casefilespecname + '/' + @name)
end
def CaseFileUri(objectmodelname, casefilespecname, name)
	URI(ITSHOST + 'casefiles/' + @objectmodelname + '/' + @casefilespecname + '/' + @name)
end

#
#	Global functions
#
def PutResource(resourceName, resourceFile, resourceUri)
	puts "PUT-ting: " + resourceName + " from file: " + resourceFile
	
	#	read the file
	file = File.new( resourceFile )
	content = file.read
	
	#	fill the arguments
	args = {}
	args[:method] = :put
	args["Content-Length"] = content.size.to_s
	args[:body] = content
	args["Content-type"] = "application/xml"
	args["From"] = FROM
	
	#	finally the actual PUT
	open(resourceUri, args)
end


#
#     put a new version of the Object Model
#
def PutObjectModel(putresource)
	$objectModel = ObjectModel.new("SimpleModel", "ObjectModel.xml")
	if putresource
		PutResource($objectModel.name, $objectModel.filename, $objectModel.uri)
	end
end
#
#	put the casefile specifications
#
def PutCaseFileSpecifications(putresource)
	$caseFileSpecList = Array.new
	$caseFileSpecList[0] = CaseFileSpec.new("Persoon", $objectModel, "CaseFileSpecPersoon.xml")   
	if putresource
		$caseFileSpecList.each { |cfs| 
			PutResource(cfs.name, cfs.filename, cfs.uri)
		}
	end
end

#
#	put the representations for Persoon
#
def PutViewsForPerson(putresource)
	_viewPersonList = Array.new
	if putresource
		_viewPersonList.each { |rs| 
			PutResource(rs.name, rs.filename, rs.uri)
		}
	end
end

#
#	Put the rulesets
#
def PutRuleSets(putresource)
	_ruleList = Array.new
	_ruleList[0] = RuleSpec.new("BerekenLeeftijd", $objectModel, $caseFileSpecList[0], "BerekenLeeftijdRule.xml")
	if putresource
		_ruleList.each { |rs| 
			PutResource(rs.name, rs.filename, rs.uri)
		}
	end
end
	
#
#	put the casefiles for Persoon
#
def PutCaseFilesForPerson
	_caseFilePersonList = Array.new
	_caseFilePersonList[0] = CaseFile.new("BSN001", $objectModel, $caseFileSpecList[0], "PersoonTheoTebockel.xml")
	_caseFilePersonList.each { |cf| 
		PutResource(cf.name, cf.filename, cf.uri)
	}
end

#------------------------------------------------------------------------
#
#	Main starts here
#
#	possible arguments: dataonly, specsonly (they both default to false)
#
#------------------------------------------------------------------------

puts "Running " + $0
ARGV.each { |option|
	if option == "dataonly" 
		putspecs = false
	end
	if option == "specsonly"
		putdata = false
	end
}

puts "Putting specifications..."
PutObjectModel(putspecs)
PutCaseFileSpecifications(putspecs)
PutViewsForPerson(putspecs)
PutRuleSets(putspecs)
puts "Putting data..."
if putdata
	PutCaseFilesForPerson()
end

puts "Hit the Enter key to end the script..."
keyboard = IO.open(0)
keyboard.gets
