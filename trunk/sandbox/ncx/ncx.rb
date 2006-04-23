#!/usr/bin/ruby

# Some quick tests in Ruby for NCX manipulation

class Audio

  def initialize(src, clip_begin, clip_end)
    @src = src
    @clipBegin = clip_begin
    @clipEnd = clip_end
  end
 
  def to_s
    "@audio(#{@src}:#{@clipBegin}:#{@clipEnd})"
  end

end

class NavLabel

  def initialize(text, audio)
    @text = text
    @audio = audio
  end

  attr_writer :text, :audio

  def NavLabel.text_only(text)
    NavLabel.new(text, nil)
  end

  def NavLabel.audio_only(audio)
    NavLabel.new(nil, audio)
  end

  def to_s
    if @text != nil 
      if @audio != nil
        "<\"#{@text}\", #{@audio}>"
      else
        "\"#{@text}\""
      end
    else
      @audio.to_s
    end
  end

end

class NavPoint

  def initialize(labels, content)
    @parent = nil
    @labels = labels
    @content = content
    @children = []
  end

  attr_reader :parent
  attr_writer :parent

  def append_child(child)
    @children.push(child)
    child.parent = self
  end

  def add_child_after(new, existing)
    @children[@children.index(existing) + 1, 0] = new
    new.parent = self
  end

  # Remove a child node and all its following siblings and return the list
  # of removed nodes
  def remove_child_and_siblings(child)
    @children.slice!(@children.index(child) .. -1)
  end

  # Try to decrease the level of an element: only works if it has no
  # children.
  def decrease_level
    p = @parent
    if p != nil and p.parent != nil and @children.empty?
      # Remove the node and its following siblings from the parent. The
      # siblings will become the children of the node after it's been
      # shifted
      removed = p.remove_child_and_siblings(self)
      # Keep only the siblings
      removed.shift
      # Move this node to its parent's parent
      p.parent.add_child_after(self, p)
      # The put back the siblings as children of this node
      # (should be append_children)
      removed.each { |child| self.append_child(child) }
      true
    else
      false
    end
  end

  def show
    _show("+ ", 1)
  end

  def _show(indent, num)
    #print("#{indent}#{num} ", @labels.join("/"), " = #{@content}\n")
    puts "#{indent}#{num} #{self}"
    @children.each { |navpoint| num = navpoint._show("  #{indent}", num + 1) }
    return num
  end

  def to_s
    @labels.join("/") + " = #{@content} < #{@parent}"
  end

end

class NavMap < NavPoint

  def initialize(navpoints)
    @parent = nil
    @children = navpoints
    @children.each { |navpoint| navpoint.parent = self }
  end
  
  def show
    num = 0
    @children.each { |navpoint| num = navpoint._show("+ ", num + 1) }
  end

  def to_s
    "navMap"
  end

end

navs = ["A", "B", "C", "D", "E", "F", "G"].map \
  { |l| NavPoint.new([NavLabel.text_only(l)], "foo.smil#" + l) }
navs[0].append_child(navs[1])
navs[1].append_child(navs[2])
navs[0].append_child(navs[3])
navs[0].append_child(navs[4])
navs[5].append_child(navs[6])
navmap = NavMap.new([navs[0], navs[5]])
navmap.show
puts "---"
if navs[3].decrease_level
  navmap.show
end
#title = NavLabel.new("Title", Audio.new("title-en.mp3", "0s", "12.2s"))
#titre = NavLabel.new("Titre", Audio.new("title-fr.mp3", "0s", "10.5s"))
#section_1 = NavLabel.text_only("Section 1")
#section_2 = NavLabel.audio_only(Audio.new("section2.mp3", "0s", "3.4s"))
#subsection_21 = NavLabel.text_only("Subsection 2.1")
#subsection_21.audio = Audio.new("sub21.mp3", "0s", "12.67s")
#n = NavPoint.new([title, titre], "ncx.smil#title")
#s1 = NavPoint.new([section_1], "ncx.smil#section_1")
#n.append_child(s1)
#s2 = NavPoint.new([section_2], "ncx.smil#section_2")
#n.append_child(s2)
#sub21 = NavPoint.new([subsection_21], "ncx.smil#subsection_2_1")
#s2.append_child(sub21)
#n.show
