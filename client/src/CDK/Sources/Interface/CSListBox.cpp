#define CDK_IMPL

#include "CSListBox.h"

CSListBox::CSListBox() {
    setLayout(CSLayerLayoutVertical);
}

bool CSListBox::addLayer(CSLayer* layer) {
    if (CSScrollPane::addLayer(layer)) {
        layer->OnTouchesBegan.add(this, &CSListBox::onTouchesBeganItem);
        return true;
    }
    return false;
}

bool CSListBox::insertLayer(int index, CSLayer* layer) {
    if (CSScrollPane::insertLayer(index, layer)) {
        layer->OnTouchesBegan.add(this, &CSListBox::onTouchesBeganItem);
        return true;
    }
    return false;
}

void CSListBox::detach(CSLayer* layer) {
    if (_selectedItem == layer) {
        onSelect(NULL);
        _selectedItem = NULL;
        
        layer->OnTouchesBegan.remove(this);
    }
}

void CSListBox::changeSelectedItem(CSLayer* selectedItem) {
    _selectionChanged = _selectedItem != selectedItem;
    
    if (_selectedItem != selectedItem) {
        if (_selectedItem) _selectedItem->cancelTouches();
        onSelect(selectedItem);
        _selectedItem = selectedItem;
        
        if (_autoFocus) focusLayer(_selectedItem);
    }
    else {
        onSelect(selectedItem);
    }
    refresh();
}

void CSListBox::setSelectedItem(CSLayer* selectedItem) {
    if (selectedItem == NULL || (layers() && layers()->containsObject(selectedItem))) {
        changeSelectedItem(selectedItem);
    }
}

int CSListBox::selectedIndex() const {
    return _selectedItem && layers() ? layers()->indexOfObject(_selectedItem) : -1;
}

void CSListBox::setSelectedIndex(int selectedIndex) {
    CSLayer* selectedItem = selectedIndex >= 0 && layers() ? layers()->objectAtIndex(selectedIndex) : NULL;
    
    changeSelectedItem(selectedItem);
}

void CSListBox::submitLayout() {
    CSLayer::submitLayout();
    
    if (_autoFocus && _selectedItem) focusLayer(_selectedItem);
}

void CSListBox::onTouchesMoved() {
    CSScrollPane::onTouchesMoved();
    
    if (focusScroll() && scroll.isScrolling()) setSelectedItem(NULL);
}

void CSListBox::onTouchesBeganItem(CSLayer* layer) {
    if (!scroll.isScrolling()) changeSelectedItem(layer);
}

void CSListBox::onSelect(CSLayer* next) {
    OnSelect(this, next);
}

