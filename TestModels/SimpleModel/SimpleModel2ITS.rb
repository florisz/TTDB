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
#	Global Variables
#
$objectModel = nil
$caseFileSpecList = nil

#
#	global declarations 
#
ITSHOST = 'http://localhost:8080/its/'
FROM = "floris.zwarteveen@luminis.nl"

#
#	Global class defintions
#
class ObjectModel
	def initialize(name, filename)
		@name = name
		@filename = filename
	end
	attr_reader :name, :filename
	def uri
		URI(ITSHOST + 'specifications/objectmodels/' + @name)
	end
end
class CaseFileSpec
	def initialize(name, objectmodel, filename)
		@name = name
		@objectmodel = objectmodel
		@filename = filename
	end
	attr_reader :name, :objectmodel, :filename
	def uri
		URI(ITSHOST + 'specifications/casefiles/' + @objectmodel.name + '/' + @name)
	end
end
class RepresentationSpec
	def initialize(name, objectmodel, casefilespec, filename)
		@name = name
		@objectmodel = objectmodel
		@casefilespec = casefilespec
		@filename = filename
	end
	attr_reader :name, :objectmodel, :casefilespec, :filename
	def uri
		URI(ITSHOST + 'representations/' + @objectmodel.name + '/' + @casefilespec.name + '/' + @name)
	end
end
class RuleSpec
	def initialize(name, objectmodel, casefilespec, filename)
		@name = name
		@objectmodel = objectmodel
		@casefilespec = casefilespec
		@filename = filename
	end
	attr_reader :name, :objectmodel, :casefilespec, :filename
	def uri
		URI(ITSHOST + 'rules/' + @objectmodel.name + '/' + @casefilespec.name + '/' + @name)
	end
end
class CaseFile
	def initialize(name, objectmodel, casefilespec, filename)
		@name = name
		@objectmodel = objectmodel
		@casefilespec = casefilespec
		@filename = filename
	end
	attr_reader :name, :objectmodel, :casefilespec, :filename
	def uri
		URI(ITSHOST + 'casefiles/' + @objectmodel.name + '/' + @casefilespec.name + '/' + @name)
	end
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
	_caseFilePersonList[1] = CaseFile.new("BSN002", $objectModel, $caseFileSpecList[0], "PersoonRichard.xml")
	_caseFilePersonList[2] = CaseFile.new("BSN003", $objectModel, $caseFileSpecList[0], "PersoonFloris.xml")
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
putspecs = true
putdata = true
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
