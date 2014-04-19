# Ruby script
#
# Put the SimpleModel specification to the TimeLine Server
#
#	includes
#
require 'rubygems'
require 'rexml/document'
require 'rest-open-uri'

#
#	global class defintions
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

#
#	global functions
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
#	global declarations 
#
ITSHOST = 'http://localhost:8080/its/'
FROM = "floris.zwarteveen@luminis.nl"
_objectModel = ObjectModel.new("Automobile", "Automobile.xml")
_caseFileSpecList = Array.new
_caseFileSpecList[0] = CaseFileSpec.new("Autohistorie", _objectModel, "Autohistorie.xml")   
_ruleList = Array.new
_ruleList[0] = RuleSpec.new("EnergyLabel", _objectModel, _caseFileSpecList[0], "EnergyLabelRule.xml")
_caseFileCarList = Array.new
_caseFileCarList[0] = CaseFile.new("09-SK-DG", _objectModel, _caseFileSpecList[0], "09SKDG.xml")
_caseFileCarList[1] = CaseFile.new("52-RV-XT", _objectModel, _caseFileSpecList[0], "52RVXT.xml")

#
#     put a new version of the Object Model
#
PutResource(_objectModel.name, _objectModel.filename, _objectModel.uri)

#
#	put the casefile specifications
#
_caseFileSpecList.each { |cfs| 
	puts "Casefile spec name: " + cfs.name
	puts "To uri: " + cfs.uri.to_s
	PutResource(cfs.name, cfs.filename, cfs.uri)
}

#
#	Put the rulesets
#
_ruleList.each { |rs| 
	PutResource(rs.name, rs.filename, rs.uri)
}

#
#	Put the casefiles
#
_caseFileCarList.each { |cf| 
	PutResource(cf.name, cf.filename, cf.uri)
}


puts "Hit a key to end the script..."
keyboard = IO.open(0)
keyboard.gets